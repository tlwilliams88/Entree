#!/usr/bin/env node
var querystring = require('querystring');
var username = '';

var util = require('util'),
    http = require('http'),
    fs = require('fs'),
    url = require('url'),
    events = require('events');

var DEFAULT_PORT = 9002;

function main(argv) {
  new HttpServer({
    'GET': createServlet(StaticServlet),
    'HEAD': createServlet(StaticServlet)
  }).start(Number(argv[2]) || DEFAULT_PORT);
}

function escapeHtml(value) {
  return value.toString().
    replace('<', '&lt;').
    replace('>', '&gt;').
    replace('"', '&quot;');
}

function createServlet(Class) {
  var servlet = new Class();
  return servlet.handleRequest.bind(servlet);
}

/**
 * An Http server implementation that uses a map of methods to decide
 * action routing.
 *
 * @param {Object} Map of method => Handler function
 */
function HttpServer(handlers) {
  this.handlers = handlers;
  this.server = http.createServer(this.handleRequest_.bind(this));
}

HttpServer.prototype.start = function(port) {
  this.port = port;
  this.server.listen(port);
  util.puts('Http Server running at http://localhost:' + port + '/');
};

HttpServer.prototype.parseUrl_ = function(urlString) {
  var parsed = url.parse(urlString);
  parsed.pathname = url.resolve('/', parsed.pathname);
  return url.parse(url.format(parsed), true);
};

HttpServer.prototype.handleRequest_ = function(req, res) {
  var logEntry = req.method + ' ' + req.url;
  if (req.headers['user-agent']) {
    logEntry += ' ' + req.headers['user-agent'];
  }

  util.puts(logEntry);
  req.url = this.parseUrl_(req.url);

  var response = null;
  var substr = req.url.path.split("?");
  var url = substr[0];
  var args = substr[1];  

  console.log('url : ' + url);

  if(req.method == 'OPTIONS'){
    res.writeHead(200, {
      "Content-Type": "application/json"
      , "Access-Control-Allow-Origin": "*"
      , "Access-Control-Allow-Methods":"POST, GET"
      , "Access-Control-Allow-Headers":"Origin, X-Requested-With, Content-Type, Accept,  Access-Control-Allow-Origin, Access-Control-Allow-Headers, Access-Control-Allow-Methods"
    });
    res.end();

  } else if(url === '/ws/user/authenticate'){
    
  } else if(url.indexOf('/products') > 0){
    writeResponse(res, getProducts());
  } else if(url.indexOf('/categories') > 0){
    writeResponse(res, getCategories());
  } else if(url.indexOf('/profile') > 0){
    writeResponse(res, getProfile());
  } else if(url.indexOf('/servicelocator') > 0){
    writeResponse(res, getEndpointUrl());
  } else {
    writeResponse(res);
  }

  
};

/**
 * Handles static content.
 */
function StaticServlet() {}

StaticServlet.MimeMap = {
  'txt': 'text/plain',
  'html': 'text/html',
  'css': 'text/css',
  'xml': 'application/xml',
  'json': 'application/json',
  'js': 'application/javascript',
  'jpg': 'image/jpeg',
  'jpeg': 'image/jpeg',
  'gif': 'image/gif',
  'png': 'image/png',
  'svg': 'image/svg+xml'
};

StaticServlet.prototype.handleRequest = function(req, res) {
  var self = this;
  var path = ('./' + req.url.pathname).replace('//','/').replace(/%(..)/g, function(match, hex){
    return String.fromCharCode(parseInt(hex, 16));
  });
  var parts = path.split('/');
  if (parts[parts.length-1].charAt(0) === '.')
    return self.sendForbidden_(req, res, path);
  fs.stat(path, function(err, stat) {
    if (err)
      return self.sendMissing_(req, res, path);
    if (stat.isDirectory())
      return self.sendDirectory_(req, res, path);
    return self.sendFile_(req, res, path);
  });
}

StaticServlet.prototype.sendError_ = function(req, res, error) {
  res.writeHead(500, {
      'Content-Type': 'text/html'
      , "Access-Control-Allow-Origin": "*"
      , "Access-Control-Allow-Methods":"POST, GET"
      , "Access-Control-Allow-Headers":"Origin, X-Requested-With, Content-Type, Accept"
  });
  res.write('<!doctype html>\n');
  res.write('<title>Internal Server Error</title>\n');
  res.write('<h1>Internal Server Error</h1>');
  res.write('<pre>' + escapeHtml(util.inspect(error)) + '</pre>');
  util.puts('500 Internal Server Error');
  util.puts(util.inspect(error));
};

StaticServlet.prototype.sendMissing_ = function(req, res, path) {
  path = path.substring(1);
  res.writeHead(404, {
      'Content-Type': 'text/html'
      , "Access-Control-Allow-Origin": "*"
      , "Access-Control-Allow-Methods":"POST, GET"
      , "Access-Control-Allow-Headers":"Origin, X-Requested-With, Content-Type, Accept"
  });
  res.write('<!doctype html>\n');
  res.write('<title>404 Not Found</title>\n');
  res.write('<h1>Not Found</h1>');
  res.write(
    '<p>The requested URL ' +
    escapeHtml(path) +
    ' was not found on this server.</p>'
  );
  res.end();
  util.puts('404 Not Found: ' + path);
};

StaticServlet.prototype.sendForbidden_ = function(req, res, path) {
  path = path.substring(1);
  res.writeHead(403, {
      'Content-Type': 'text/html'
      , "Access-Control-Allow-Origin": "*"
      , "Access-Control-Allow-Methods":"POST, GET"
      , "Access-Control-Allow-Headers":"Origin, X-Requested-With, Content-Type, Accept"
  });
  res.write('<!doctype html>\n');
  res.write('<title>403 Forbidden</title>\n');
  res.write('<h1>Forbidden</h1>');
  res.write(
    '<p>You do not have permission to access ' +
    escapeHtml(path) + ' on this server.</p>'
  );
  res.end();
  util.puts('403 Forbidden: ' + path);
};

StaticServlet.prototype.sendRedirect_ = function(req, res, redirectUrl) {
  res.writeHead(301, {
      'Content-Type': 'text/html'
      , 'Location': redirectUrl
      , "Access-Control-Allow-Origin": "*"
      , "Access-Control-Allow-Methods":"POST, GET"
      , "Access-Control-Allow-Headers":"Origin, X-Requested-With, Content-Type, Accept"
  });
  res.write('<!doctype html>\n');
  res.write('<title>301 Moved Permanently</title>\n');
  res.write('<h1>Moved Permanently</h1>');
  res.write(
    '<p>The document has moved <a href="' +
    redirectUrl +
    '">here</a>.</p>'
  );
  res.end();
  util.puts('301 Moved Permanently: ' + redirectUrl);
};

StaticServlet.prototype.sendFile_ = function(req, res, path) {
  var self = this;
  var file = fs.createReadStream(path);
  res.writeHead(200, {
      "Access-Control-Allow-Origin": "*"
      , "Access-Control-Allow-Methods":"POST, GET"
      , "Access-Control-Allow-Headers":"Origin, X-Requested-With, Content-Type, Accept"
      ,'Content-Type': StaticServlet.
      MimeMap[path.split('.').pop()] || 'text/plain'
  });
  if (req.method === 'HEAD') {
    res.end();
  } else {
    file.on('data', res.write.bind(res));
    file.on('close', function() {
      res.end();
    });
    file.on('error', function(error) {
      self.sendError_(req, res, error);
    });
  }
};

StaticServlet.prototype.sendDirectory_ = function(req, res, path) {
  var self = this;
  if (path.match(/[^\/]$/)) {
    req.url.pathname += '/';
    var redirectUrl = url.format(url.parse(url.format(req.url)));
    return self.sendRedirect_(req, res, redirectUrl);
  }
  fs.readdir(path, function(err, files) {
    if (err)
      return self.sendError_(req, res, error);

    if (!files.length)
      return self.writeDirectoryIndex_(req, res, path, []);

    var remaining = files.length;
    files.forEach(function(fileName, index) {
      fs.stat(path + '/' + fileName, function(err, stat) {
        if (err)
          return self.sendError_(req, res, err);
        if (stat.isDirectory()) {
          files[index] = fileName + '/';
        }
        if (!(--remaining))
          return self.writeDirectoryIndex_(req, res, path, files);
      });
    });
  });
};

StaticServlet.prototype.writeDirectoryIndex_ = function(req, res, path, files) {
  path = path.substring(1);
  res.writeHead(200, {
    'Content-Type': 'text/html'
      , "Access-Control-Allow-Origin": "*"
      , "Access-Control-Allow-Methods":"POST, GET"
      , "Access-Control-Allow-Headers":"Origin, X-Requested-With, Content-Type, Accept"
  });
  if (req.method === 'HEAD') {
    res.end();
    return;
  }
  res.write('<!doctype html>\n');
  res.write('<title>' + escapeHtml(path) + '</title>\n');
  res.write('<style>\n');
  res.write('  ol { list-style-type: none; font-size: 1.2em; }\n');
  res.write('</style>\n');
  res.write('<h1>Directory: ' + escapeHtml(path) + '</h1>');
  res.write('<ol>');
  files.forEach(function(fileName) {
    if (fileName.charAt(0) !== '.') {
      res.write('<li><a href="' +
        escapeHtml(fileName) + '">' +
        escapeHtml(fileName) + '</a></li>');
    }
  });
  res.write('</ol>');
  res.end();
};

var getProducts = function(){

  return JSON.stringify({"products":[{"id":"101285","description":"Shrimp Raw Hdls 25/30","ext_description":"Premium Wild Texas White","brand":"Cortona","size":"5 LB","upc":"00000000000000","manufacturer_number":"B-W-26/30","manufacturer_name":"Ellington Farms Seafood","cases":"0","categoryId":"FS490","kosher":"true","price":"325.00"},{"id":"101286","description":"Crawfish Raw Hdls 25/30","ext_description":"Premium Wild Texas White","brand":"Markon Cooperative","size":"5 LB","upc":"00000000000000","manufacturer_number":"B-W-26/30","manufacturer_name":"Philly Seafood","cases":"1","categoryId":"FS490","kosher":"true","price":"325.00"},{"id":"101186","description":"Shrimp Cooked Hdls 26/30","ext_description":"Premium Wild Texas White","brand":"Philly","size":"5 LB","upc":"00000000000000","manufacturer_number":"B-W-26/30","manufacturer_name":"Philly Seafood","cases":"4","categoryId":"FS490","kosher":"true","price":"325.00"},{"id":"101386","description":"Shrimp Raw Hdls 26/30","ext_description":"Premium Wild Texas White","brand":"Philly","size":"5 LB","upc":"00000000000000","manufacturer_number":"B-W-26/30","manufacturer_name":"Philly Seafood","cases":"3","categoryId":"FS490","kosher":"true","price":"325.00"},{"id":"101385","description":"Shrimp Raw Hdls 26/30","ext_description":"Premium Wild Texas White","brand":"Philly","size":"5 LB","upc":"00000000000000","manufacturer_number":"B-W-26/30","manufacturer_name":"Philly Seafood","cases":"2","categoryId":"FS490","kosher":"true","price":"325.00"},{"id":"101384","description":"Shrimp Raw Hdls 26/30","ext_description":"Premium Wild Texas White","brand":"Philly","size":"5 LB","upc":"00000000000000","manufacturer_number":"B-W-26/30","manufacturer_name":"Philly Seafood","cases":"0","categoryId":"FS490","kosher":"true","price":"325.00"}]});
};

var getCategories = function() {
  return JSON.stringify({"categories":[{"name":"Shrimp","description":"Shrimp, Headless, Raw","id":"FS940","subcategories":[{"id":"FS942","name":"Jumbo Shrimp","description":"Shrimp, Headless, Raw 12 Ct"},{"id":"FS941","name":"Popcorn Shrimp","description":"Shrimp, Headless, Raw 65 Ct"}]},{"name":"Cakes","description":"Cakes Silly","id":"CS940","subcategories":[{"id":"CS942","name":"Cakes, Decorated","description":"Decorated Cakes"},{"id":"CS941","name":"Cakes, Bulk","description":"Un-decorated cakes"}]},{"name":"Pork","description":"Soo weee!!!","id":"PS940","subcategories":[{"id":"PS942","name":"Pork Shoulder","description":"Bulk Pork Shoulder"},{"id":"PS941","name":"Pork, Chops","description":"Pork Chops"}]}]});
}

var getProfile = function() {
  return JSON.stringify({"profile": {
    "name": "Steven Broussard",
    "customerNumber": 12345,
    "imageUrl": null,
    "role": null,
    "phone": 9348234934,
    "location": "Dallas",
    "stores": [{
      "name": "Chick-fil-A",
      "customerNumber": 453234
    }, {
      "name": "Saltgrass",
      "customerNumber": 534939
    }],
    "accountNumber": 9783459,
    "salesRep": {
      "id": 34234,
      "name": "Heather Hill",
      "phone": 8889122342,
      "email": "heather.hill@ben.e.keith.com",
      "imageUrl": null
    }
  }});
}

var syncActivities = function(){
  return JSON.stringify({Response:"200", Error:""});
}

var authenticateBRubble = function(path){
  // var sites = [{ siteId: 1, siteName: "Jacksonville" }, { siteId: 2, siteName: "Cherry Point" }];
  var sites = [{ siteId: 1001, siteName: "Jacksonville" }];
  return JSON.stringify({"token":"612dcf04-24a7-45bb-94ae-7242fc24cd84","userSites":sites,"expiration":1397588993536,"username":"brubble"});
}

var authenticateFFlinstone = function(path){
  // var sites = [{ siteId: 1, siteName: "Jacksonville" }, { siteId: 2, siteName: "Cherry Point" }];
  var sites = [{ siteId: 1001, siteName: "New York" },{siteId: 1002, siteName:"San Francisco"}];
  return JSON.stringify({"token":"612dcf04-24a7-45bb-94ae-7242fc24cd84","userSites":sites,"expiration":1397588993536,"username":"fflinstone"});
}

var  getEndpointUrl = function() {
  return JSON.stringify({"ClientApiEndpoint":"localhost:49420"});
}

var writeResponse = function(res, content){
  if(!content ) {
    console.log('Path Not defined');
    res.writeHead(400, {
      "Content-Type": "application/json"
      , "Access-Control-Allow-Origin": "*"
      , "Access-Control-Allow-Methods":"POST, GET"
      , "Access-Control-Allow-Headers":"Origin, X-Requested-With, Content-Type, Accept,  Access-Control-Allow-Origin, Access-Control-Allow-Headers, Access-Control-Allow-Methods"
    });
    res.write('Internal Server Error : Path not defined');
  } else {
    res.writeHead(200, {
      "Content-Type": "application/json"
      , "Content-Length":content.length
      , "Access-Control-Allow-Origin": "*"
      , "Access-Control-Allow-Methods":"POST, GET"
      , "Access-Control-Allow-Headers":"Origin, X-Requested-With, Content-Type, Accept,  Access-Control-Allow-Origin, Access-Control-Allow-Headers, Access-Control-Allow-Methods"
    });

    console.log('response found ' + content);
    res.write(content);
  }
  res.end();
}

var logoff = function(){
  return JSON.stringify({Response:"200", Error:""});
}

console.log('MockAPI running on 127.0.0.1:' + DEFAULT_PORT);

// Must be last,
main(process.argv);
