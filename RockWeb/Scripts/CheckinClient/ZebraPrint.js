//
//  ZebraPrint.js
//  Zebra Print plugin for Cordova iOS and Windows App
//
//  Copyright 2013 Spark Development Network. 
//


var ZebraPrintPlugin = {

    // print tags
    printTags: function (tagJson, success, fail) {

        // do some logic to clean up null values
        var labels = JSON.parse(tagJson);

        for (var label in labels) {
            if (labels[label].PrinterDeviceId == null) {
                labels[label].PrinterDeviceId = 0;
            }

            // clean up null printer address
            if (labels[label].PrinterAddress == null) {
                labels[label].PrinterAddress = "";
            }
        }

        tagJson = JSON.stringify(labels);
        if (labels.length > 0) {
            // call plugins
            if (navigator.userAgent.match(/(iPhone|iPod|iPad)/)) {
                console.log('Rock iPad Client');
                Cordova.exec(success, fail, "ZebraPrint", "printTags", [tagJson]);
            } else if (navigator.userAgent.match(/rockwinclient/)) {
                console.log('Rock Windows Client 2.0');
                eoWebBrowser.extInvoke("printLabels", [tagJson]);
            } else if (navigator.userAgent.match(/.NET CLR/)) {
                console.log('Rock Windows Client 1.0');
                window.external.PrintLabels(tagJson);
            } else {
                console.log('Rock Windows Client 3.0');
                window.external.PrintLabels(tagJson);
            }
        }
    }
};
