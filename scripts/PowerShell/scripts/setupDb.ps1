Param ([string] $connectionString, [string] $scriptPath)

# open SQL Connection
$connection = New-Object System.Data.SqlClient.SqlConnection
$connection.ConnectionString = $connectionString
$connection.Open()

# load script file
$script = Get-Content $scriptPath

# execute command
$command = $connection.CreateCommand()
$command.CommandType = [System.Data.CommandType]::Text
$command.CommandText = $script
$command.ExecuteReader()

# close connection
$connection.Close()
