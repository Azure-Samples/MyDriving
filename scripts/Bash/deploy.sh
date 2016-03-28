#!/bin/bash
set -e
set -o pipefail

# Variables
DEPLOYMENTNAME="scenario_complete-$(date -u +%m%d-%H%M)"
TEMPLATEFILE="../ARM/scenario_complete.nocomments.json"
PREREQ_TEMPLATEFILE="../ARM/prerequisites.nocomments.json"
PARAMETERSFILE="../ARM/scenario_complete.params.nocomments.json"
ASSETS_DIRECTORY="../../src/HDInsight"
SQLSERVER_DDL_SCRIPT="../../src/SQLDatabase/MyDrivingDB.sql"
SQLANALYTICS_DDL_SCRIPT="../../src/SQLDatabase/MyDrivingAnalyticsDB.sql"

# Parse parameters, use > 1 to consume two arguments per pass in the loop.
while [[ $# > 1 ]] 
do
    key="$1"

    case $key in
        -n|--name)
        RESOURCEGROUPNAME="$2"
        shift
        ;;
        -l|--location)
        LOCATION="$2"
        shift
        ;;
        *)
        # unknown option
        ;;
    esac
    shift # past argument or value
done

# validate resource group was provided
if [ -z ${RESOURCEGROUPNAME+x} ]; then
    echo "Please provide the resource name with the -n|--name parameter."
    exit
fi

# validate resource group location was provided
if [ -z ${LOCATION+x} ]; then
    echo "Please provide the resource location with the -l|--location parameter."
    echo "You can get the list of supported locations with the command azure location list"
    exit
fi

# verify if user is logged in by querying his subscriptions.
# if none is found assume he is not
echo
echo "**************************************************************************************************"
echo "* Retrieving Azure subscription information..."
echo "**************************************************************************************************"
IFS=$'\n'
SUBSCRIPTIONS=($(azure account list | awk -F $'  ' '{printf $3"\n"}'))
unset IFS

# azure account list outputs 2 lines by default if no subscription is found
# so an array of length 2 means no subscription was found
if [ ${#SUBSCRIPTIONS[@]} -eq 2 ]; then
    echo "login"
    azure login
    IFS=$'\n'
    SUBSCRIPTIONS=($(azure account list | awk -F $'  ' '{printf $3"\n"}'))
    unset IFS
fi

# fail if we still can retrieve any subscription
if [ ${#SUBSCRIPTIONS[@]} -eq 2 ]; then
    echo "Login failed or there are no subscriptions available with your account."
    echo "Please logout using the command azure logout --user [username] and try again."
    exit
fi

# if the user has more than one subscriptions force the user to select one
# azure account list outputs 4 lines by default if a subscription is found
# so an array of length 5 means 1 subscription was found
if [ ${#SUBSCRIPTIONS[@]} -gt 5 ]; then
    #remove last element of array
    unset SUBSCRIPTIONS[${#SUBSCRIPTIONS[@]}-1]
    
    #remove first three elements of array and compact array
    SUBSCRIPTIONS=( "${SUBSCRIPTIONS[@]:3}" )
    
    select sub in "${SUBSCRIPTIONS[@]}"
    do
        if [ 1 -le "$REPLY" ] && [ "$REPLY" -le ${#SUBSCRIPTIONS[@]} ]; then
            azure account set "$sub"
            break;
        else 
            echo "Invalid selection, please choose a number from 1 to ${#SUBSCRIPTIONS[@]}"
        fi
    done
fi 

# switch to Azure Resource Manager mode
azure config mode arm

# create the resource group
echo
echo "**************************************************************************************************"
echo "* Creating the resource group..."
echo "**************************************************************************************************"
azure group create --name "${RESOURCEGROUPNAME}" --location "${LOCATION}"

# create the deployment with the prerequisites template
echo
echo "**************************************************************************************************"
echo "* Deploying the prerequisites..."
echo "**************************************************************************************************"
if ! OUTPUT0=$(azure group deployment create \
					--name "${DEPLOYMENTNAME}-0" \
					--resource-group "${RESOURCEGROUPNAME}" \
					--template-file "${PREREQ_TEMPLATEFILE}" \
				| tee /dev/tty); then
	echo "Failed to provision the storage account for the prerequisites. Review the output above to correct any errors and then run the deployment script again."
	exit 1;
fi

# parse the deployment output
eval $(echo "$OUTPUT0" | awk -F ': ' \
               '$2 ~ /storageAccountName/  {split($2, a, " "); print "export AZURE_STORAGE_ACCOUNT="    a[3] ";"} \
                $2 ~ /storageAccountKey/   {split($2, a, " "); print "export AZURE_STORAGE_ACCESS_KEY=" a[3] ";"} \
                $2 ~ /assetsContainerName/ {split($2, a, " "); print "export ASSETS_CONTAINER_NAME="    a[3] ";"}')

echo
echo "**************************************************************************************************"
echo "* Uploading files to blob storage..."
echo "**************************************************************************************************"
# uploads files in a specified directory
uploadAssets() {
 for i in "$1"/*;do
    if [ -d "$i" ];then
        uploadAssets "$i"
    elif [ -f "$i" ]; then
        blobName=${i: ${#ASSETS_DIRECTORY} + 1}
        echo "Uploading file '$blobName' to blob storage..."
        azure storage blob upload -q $i $ASSETS_CONTAINER_NAME $blobName
    fi
 done
}

# create the storage account container for the assets
echo "Creating the '$ASSETS_CONTAINER_NAME' blob container to store assets..."
azure storage container create $ASSETS_CONTAINER_NAME || :

# upload the assets to the container
echo "Copying the assets in '$ASSETS_DIRECTORY' to blob storage..."
uploadAssets $ASSETS_DIRECTORY

# create the deployment with the solution template
echo
echo "**************************************************************************************************"
echo "* Deploying the resources in the ARM template. This operation may take several minutes..."
echo "**************************************************************************************************"
if ! OUTPUT1=$(azure group deployment create \
					--name "${DEPLOYMENTNAME}-1" \
					--resource-group "${RESOURCEGROUPNAME}" \
                    --template-file "${TEMPLATEFILE}" \
                    --parameters-file "${PARAMETERSFILE}" \
				| tee /dev/tty); then
	echo "Skipping the storage and database initialization..."
	echo "At least one resource could not be provisioned successfully. Review the output above to correct any errors and then run the deployment script again."
	exit 2;
fi

# parse the deployment output
eval $(echo "$OUTPUT1" | awk -F ': ' \
               '$2 ~ /sqlServerFullyQualifiedDomainName/    {split($2, a, " "); print "export SQLSERVER_FULLY_QUALIFIED_DOMAIN_NAME="    a[3] ";"} \
                $2 ~ /sqlServerAdminLogin/                  {split($2, a, " "); print "export SQLSERVER_ADMIN_LOGIN="                    a[3] ";"} \
                $2 ~ /sqlServerAdminPassword/               {split($2, a, " "); print "export SQLSERVER_ADMIN_PASSWORD="                 a[3] ";"} \
                $2 ~ /sqlDBName/                            {split($2, a, " "); print "export SQLSERVER_DATABASE_NAME="                  a[3] ";"} \
                $2 ~ /sqlAnalyticsFullyQualifiedDomainName/ {split($2, a, " "); print "export SQLANALYTICS_FULLY_QUALIFIED_DOMAIN_NAME=" a[3] ";"} \
                $2 ~ /sqlAnalyticsServerAdminLogin/         {split($2, a, " "); print "export SQLANALYTICS_ADMIN_LOGIN="                 a[3] ";"} \
                $2 ~ /sqlAnalyticsServerAdminPassword/      {split($2, a, " "); print "export SQLANALYTICS_ADMIN_PASSWORD="              a[3] ";"} \
                $2 ~ /sqlAnalyticsDBName/                   {split($2, a, " "); print "export SQLANALYTICS_DATABASE_NAME="               a[3] ";"} \
                $2 ~ /storageAccountNameAnalytics/          {split($2, a, " "); print "export STORAGE_ACCOUNT_NAME_ANALYTICS="           a[3] ";"} \
				$2 ~ /storageAccountKeyAnalytics/           {split($2, a, " "); print "export STORAGE_ACCOUNT_KEY_ANALYTICS="            a[3] ";"} \
				$2 ~ /rawdataContainerName/                 {split($2, a, " "); print "export RAW_DATA_CONTAINER_NAME="                  a[3] ";"} \
				$2 ~ /tripdataContainerName/                {split($2, a, " "); print "export TRIP_DATA_CONTAINER_NAME="                 a[3] ";"} \
				$2 ~ /referenceContainerName/               {split($2, a, " "); print "export REFERENCE_CONTAINER_NAME="                 a[3] ";"}')

echo
echo "**************************************************************************************************"
echo "* Initializing blob storage..."
echo "**************************************************************************************************"
# create the storage account container for raw data
echo "Creating the '$RAW_DATA_CONTAINER_NAME' blob container..."
azure storage container create $RAW_DATA_CONTAINER_NAME -a $STORAGE_ACCOUNT_NAME_ANALYTICS -k $STORAGE_ACCOUNT_KEY_ANALYTICS || :

# create the storage account container for trip data
echo "Creating the '$TRIP_DATA_CONTAINER_NAME' blob container..."
azure storage container create $TRIP_DATA_CONTAINER_NAME -a $STORAGE_ACCOUNT_NAME_ANALYTICS -k $STORAGE_ACCOUNT_KEY_ANALYTICS || :

# create the storage account container for reference data
echo "Creating the '$REFERENCE_CONTAINER_NAME' blob container..."
azure storage container create $REFERENCE_CONTAINER_NAME -a $STORAGE_ACCOUNT_NAME_ANALYTICS -k $STORAGE_ACCOUNT_KEY_ANALYTICS || :

# Initialize SQL databases
echo
echo "**************************************************************************************************"
echo "* Preparing the SQL databases..."
echo "**************************************************************************************************"
# set up the SQL Database
node setupDb $SQLSERVER_FULLY_QUALIFIED_DOMAIN_NAME $SQLSERVER_ADMIN_LOGIN $SQLSERVER_ADMIN_PASSWORD $SQLSERVER_DATABASE_NAME $SQLSERVER_DDL_SCRIPT

# set up the Analytics Database
node setupDb $SQLANALYTICS_FULLY_QUALIFIED_DOMAIN_NAME $SQLANALYTICS_ADMIN_LOGIN $SQLANALYTICS_ADMIN_PASSWORD $SQLANALYTICS_DATABASE_NAME $SQLANALYTICS_DDL_SCRIPT

echo
echo "The deployment is complete!"

