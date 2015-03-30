/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
var app = {
    // Application Constructor
    initialize: function() {
        this.bindEvents();
    },
    // Bind Event Listeners
    //
    // Bind any events that are required on startup. Common events are:
    // 'load', 'deviceready', 'offline', and 'online'.
    bindEvents: function() {
        document.addEventListener('deviceready', this.onDeviceReady, false);
        document.addEventListener('online', this.onOnline, false);
        document.addEventListener('offline', this.onOffline, false);
        document.addEventListener('pause', this.onPause, false);
        document.addEventListener('resume', this.onResume, false);
    },
    // deviceready Event Handler
    //
    // The scope of 'this' is the event. In order to call the 'receivedEvent'
    // function, we must explicitly call 'app.receivedEvent(...);'
    onDeviceReady: function() {
        console.log("Device Ready!");
        app.receivedEvent('deviceready');
    },
    // Update DOM on a Received Event
    receivedEvent: function(id) {
        var parentElement = document.getElementById(id);
        //var listeningElement = parentElement.querySelector('.listening');
        //var receivedElement = parentElement.querySelector('.received');

        //listeningElement.setAttribute('style', 'display:none;');
        //receivedElement.setAttribute('style', 'display:block;');

        console.log('Received Event: ' + id);
    },
    onOnline: function(){
        console.log("BACK ONLINE!");
        var listService = angular.element(jQuery('body')).injector().get('ListService');
        var cartService = angular.element(jQuery('body')).injector().get('CartService');
        var toaster = angular.element(jQuery('body')).injector().get('toaster');
        toaster.pop('success', null, "You are now connected to the server.");
        //merge local with server data
        listService.updateListsFromLocal();
        cartService.updateCartsFromLocal();

        //check for pending orders and submit if pending connection
    },
    onOffline: function(){
        console.log("Taken Offline");
        var toaster = angular.element(jQuery('body')).injector().get('toaster');
        toaster.pop('warning', null, "You are now offline.");

        var cartService = angular.element(jQuery('body')).injector().get('CartService'); 
        cartService.updateNetworkStatus();
    },
    onPause: function() {
        console.log("Paused.")
    },
    onResume: function(){
        console.log("Resumed!!");
    }

};
