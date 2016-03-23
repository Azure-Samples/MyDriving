Param ([string] $connectionString, [string] $scriptPath)

# open SQL Connection
$connection = New-Object System.Data.SqlClient.SqlConnection
$connection.ConnectionString = $connectionString
$connection.Open()

# load script file
$script = Get-Content $scriptPath -Raw

# execute command batch
$batch = $script -split "\s*GO\s+", 0, "multiline" 
$batch | 
    foreach {
        if ($_.Trim().Length -gt 0) {
		    $command = $connection.CreateCommand()
		    $command.CommandType = [System.Data.CommandType]::Text
		    $command.CommandText = $_
		    $reader = $command.ExecuteReader()
		    $reader.Close()
        }
    }

# close connection
$connection.Close()
