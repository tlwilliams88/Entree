'use strict';

angular.module('bekApp')
  .factory('PhonegapDbService', [ '$q',
    function($q) {

  //
  var dbName = 'bek_offline_storage';

  var db = null;

  function openDB() {
    if (!db) {
      db = window.openDatabase(dbName, '1.0', 'Ben E Keith Entree', 1000000); 
    }
    return db;
  };

  function convertKeyToString(key) {
    if (typeof key !== 'string') {
      key = key.toString();
    }
    return key;
  }

  var Service = {

    dropTable: function(table) {
      console.log('PhonegapDbService: drop table ' + table);
      var deferred = $q.defer();
      var db = openDB();
      db.transaction(function(tx) {
        return tx.executeSql('drop table if exists ' + table + ';', [], function(tx, res) {          
          return deferred.resolve(true);
        });
      });
      return deferred.promise;
    },

    getAllItems: function(table) {
      console.log('PhonegapDbService: get all items');
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
            console.log('found ' + items.length + ' items');

            return deferred.resolve(items);
          } else {
            return deferred.resolve(false);
          }
        });
      });
      return deferred.promise;
    },
    getItem: function(table, key) {
      console.log('PhonegapDbService: get item');
      var deferred = $q.defer();
      var db = openDB();
      key = convertKeyToString(key);
      db.transaction(function(tx) {
        // tx.executeSql('create table if not exists ' + table + ' (id integer primary key, key text, data text)');
        return tx.executeSql("select data from " + table + " where key='" + key + "';", [], function(tx, res) {
          if (res.rows.length > 0) {
            var item = JSON.parse(res.rows.item(0).data);
            console.log('found item');
            console.log(item);
            return deferred.resolve(item);
          } else {
            return deferred.resolve(false);
          }
        });
      });
      return deferred.promise;
    },
    setItem: function(table, key, data) {
      console.log('PhonegapDbService: set item');
      data = JSON.stringify(data);
      key = convertKeyToString(key);
      var db = openDB();
      db.transaction(function(tx) {
        tx.executeSql('create table if not exists ' + table + ' (id integer primary key, key text, data text)');
        return tx.executeSql("select data from " + table + " where key='" + key + "';", [], function(tx, res) {
          if (res.rows.length > 0) {
            console.log('update');
            return tx.executeSql("UPDATE " + table + " SET data = '" + data + "' WHERE key ='" + key + "'", [], function(tx, res) {
              return true;
            });
          } else {
            console.log('insert');
            return tx.executeSql("INSERT INTO " + table + " (key, data) VALUES (?,?)", [key, data], function(tx, res) {
              return true;
            });
          }
        });
      });
      return false;
    },
    removeItem: function(table, key) {
      var db = openDB();
      key = convertKeyToString(key);
      db.transaction(function(tx) {
        return tx.executeSql("DELETE FROM " + table + " WHERE key = '" + key + "';", [], function(tx, res) {
          return true;
        });
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