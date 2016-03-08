#!/bin/bash
set -e

# Variables
DEPLOYMENTNAME="scenario_complete-$(date -u +%m%d-%H%M)"
TEMPLATEFILE="../ARM/scenario_complete.json"
PARAMETERSFILE="../ARM/scenario_complete.params.json"

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
azure group create --name "${RESOURCEGROUPNAME}" --location "${LOCATION}" --verbose

# create the deployment with the provided template
azure group deployment create --name "${DEPLOYMENTNAME}" \
                              --resource-group "${RESOURCEGROUPNAME}" \
                              --template-file "${TEMPLATEFILE}" \
                              --parameters-file "${PARAMETERSFILE}" \
                              --verbose