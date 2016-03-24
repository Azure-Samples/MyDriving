var fs = require('fs');

var tedious = require('tedious'),
    Connection = tedious.Connection,
    Request = tedious.Request;

function executeQuery(connection, src, callback) {
    fs.readFile(src, 'utf8', function (err, script) {
        if (err) {
            return callback(err);
        }

        var batch = script.split(/^GO\r?\n/m);        
        function iterator(i) {
            if (i < batch.length ) {            
                var stmt = batch[i];
                var request = new Request(stmt, function(err, rowCount) {
                    if ( err ) {
                        callback(err);
                    }
                                        
                    iterator(i + 1);
                });
                                            
                connection.execSql(request);              
            }
            else {
                callback();
            }
        }
        
        iterator(0);
    });
}

function setupDb(config, scriptFiles, callback) {
    var connection = new Connection(config);
    connection.on('connect', function(err) {
        if (err) {
            return callback(err);
        }
        
        function iterator(i) {
            if (i < scriptFiles.length ) {            
                return executeQuery(connection, scriptFiles[i], function (err, rowCount) {
                    if ( err ) {
                        callback(err);
                    }
                    else {
                        // console.log("Execute query. " + rowCount + ' rows were returned.');                    
                    }
                    
                    iterator(i + 1);
                })
            }
            
            connection.close();
            callback();
        }

        iterator(0);
    });
}

var config = {
    server:   process.argv[2],
    userName: process.argv[3],
    password: process.argv[4],
    // Microsoft Azure requires the following:
    options: {encrypt: true, database: process.argv[5]}
};

// array, can handle more than one script file
var scriptFiles = [process.argv[6]];

console.log('Initializing the ' + config.options.database + ' database...');
setupDb(config, scriptFiles, function (err) {
    if (err) {
       return console.log('Error executing a database script. ' + err.message);
    }
});
