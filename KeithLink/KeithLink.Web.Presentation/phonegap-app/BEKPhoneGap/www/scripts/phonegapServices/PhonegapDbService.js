'use strict';

angular.module('bekApp')
  .factory('PhonegapDbService', [ '$q', '$log',
    function($q, $log) {

  //
  var dbName = 'bek_offline_storage';

  var db = null;

  function openDB() {
    if (!db) {
      db = window.openDatabase(dbName, '1.0', 'Ben E Keith Entree', 1000000, function() {
        $log.debug('opened database');
      }); 
    }
    return db;
  };

  function convertKeyToString(key) {
    if (typeof key !== 'string') {
      key = key.toString();
    }
    return key;
  }

  function errorHandler(tx, error) {
    $log.debug('DB error');
    debugger;
    $log.debug(error);
  }

  var Service = {

    dropTable: function(table) {
      $log.debug('PhonegapDbService: drop table ' + table);
      var deferred = $q.defer();
      var db = openDB();
      db.transaction(function(tx) {
        return tx.executeSql('drop table if exists ' + table + ';', [], function(tx, res) {
          $log.debug('drop table success');
          return deferred.resolve(true);
        }, errorHandler);
      });
      return deferred.promise;
    },

    getAllItems: function(table) {
      $log.debug('PhonegapDbService: get all items');
      var deferred = $q.defer();
      var db = openDB();
      db.transaction(function(tx) {
        tx.executeSql('create table if not exists ' + table + ' (id integer primary key, key text, data text)');
        return tx.executeSql("select data from " + table + ";", [], function(tx, res) {
          if (res.rows.length > 0) {
            var items = [];
            for (var i = 0; i < res.rows.length; i++) {
              var item = JSON.parse(res.rows.item(i).data);
              items.push(item);
            }
            $log.debug('found ' + items.length + ' items');

            return deferred.resolve(items);
          } else {
            return deferred.resolve(false);
          }
        }, errorHandler);
      });
      return deferred.promise;
    },
    getItem: function(table, key) {
      $log.debug('PhonegapDbService: get item');
      var deferred = $q.defer();
      var db = openDB();
      key = convertKeyToString(key);
      db.transaction(function(tx) {
        // tx.executeSql('create table if not exists ' + table + ' (id integer primary key, key text, data text)');
        return tx.executeSql("select data from " + table + " WHERE key = ?;", [key], function(tx, res) {
          if (res.rows.length > 0) {
            var item = JSON.parse(res.rows.item(0).data);
            $log.debug('found item');
            $log.debug(item);
            return deferred.resolve(item);
          } else {
            return deferred.resolve(false);
          }
        }, errorHandler);
      });
      return deferred.promise;
    },
    setItem: function(table, key, data) {
      $log.debug('PhonegapDbService: set item');
      data = JSON.stringify(data);
      key = convertKeyToString(key);
      var db = openDB();
      db.transaction(function(tx) {
        tx.executeSql('create table if not exists ' + table + ' (id integer primary key, key text, data text)');
        return tx.executeSql("select data from " + table + " where key=?;", [key], function(tx, res) {
          if (res.rows.length > 0) {
            $log.debug('update');
            return tx.executeSql("UPDATE " + table + " SET data = ? WHERE key = ?;", [data, key], function(tx, res) {
              $log.debug('update success');
              return true;
            }, errorHandler);
          } else {
            $log.debug('insert');
            return tx.executeSql("INSERT INTO " + table + " (key, data) VALUES (?,?)", [key, data], function(tx, res) {
              $log.debug('insert success');
              return true;
            }, errorHandler);
          }
        });
      });
      return false;
    },
    removeItem: function(table, key) {
      var db = openDB();
      key = convertKeyToString(key);
      db.transaction(function(tx) {
        return tx.executeSql("DELETE FROM " + table + " WHERE key = ?;", [key], function(tx, res) {
          return true;
        }, errorHandler);
      });
      return false;
    }
    // ,
    // removeAll: function(table) {
    //   var db = openDB();
    //   db.transaction(function(tx) {
    //     return tx.executeSql("DROP TABLE IF" + table, [], function(tx, res) {
    //       return true;
    //     });
    //   });
    //   return false;
    // }
  };

  return Service;
}]);