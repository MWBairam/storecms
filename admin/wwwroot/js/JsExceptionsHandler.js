

window.onerror = function (msg, url, lineNo, columnNo, error) {

    var message = [
        'Message: ' + msg,
        'URL: ' + url,
        'Line: ' + lineNo,
        'Column: ' + columnNo,
        'Error object: ' + JSON.stringify(error)
    ].join(' - '); //The join() method creates and returns a new string by concatenating all of the elements in an array (or an array-like object), separated by commas or a specified separator string.

    console.log(message);
    //$.notify('JS Excception !' , { globalPosition: 'top-center', className: 'error' });
    toastr.error('JS Excception !');
}

//or we can use window.addEventListener('error', function (event) { ... })
//or element.onerror = function(event) { ... }
//Refrences: 
//https://developer.mozilla.org/en-US/docs/Web/API/GlobalEventHandlers/onerror
//https://blog.sentry.io/2016/01/04/client-javascript-reporting-window-onerror

/*
msg – The message associated with the error, e.g. “Uncaught ReferenceError: foo is not defined”
url – The URL of the script or document associated with the error, e.g. “/dist/app.js”
lineNo – The line number (if available)
columnNo – The column number (if available)
error – The Error object associated with this error (if available)
*/