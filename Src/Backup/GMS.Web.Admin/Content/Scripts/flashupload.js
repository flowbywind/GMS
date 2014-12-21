//使用方法：
//1.页脚引入：
//<script src="<%=Url.StaticFile("/Content/Scripts/flashupload.js") %>" type="text/javascript"></script>
//2.页面插入元素：
//<div id="uploader"></div>
//3.上传JS代码事例：
/*
    $('#uploader').FileUpload({
        'subfolder': 'unit', 
        'callback' : function(msg) {
            //msg.url为图片绝对地址
        }
    });

    //需要定制化请使用下面的：
    $('#uploader').FileUpload({
        'subfolder': 'unit', 
        'thumbs': 's,40,30,Cut|m,500,340,Cut',
        'width': 66,
        'height': 22,
        'buttonImage': staticFileRoot + '/Common/Uploadify/uploadbutton.png',
        'method': 'GET',
        'queueID': 'fileQueue',
        'fileSizeLimit': '5MB',
        'fileTypeDesc': "jpg/png/gif Files",
        'fileTypeExts': '*.gif; *.jpg; *.png',
        'auto': true,
        'multi': true,
        'onUploadStart' : function(file) {
            alert('Starting to upload ' + file.name);
        },
        'onSelect' : function(file) {
            alert('The file ' + file.name + ' was added to the queue.');
        },
        'callback' : function(msg) {
            alert("原始图片：" + msg.url + "\r\n"
                + "m缩略图：" + getthumpath(msg.url, "m") + "\r\n"
                + "s缩略图：" + getthumpath(msg.url, "s"));
        }
    });
*/

if ("undefined" != typeof staticFileRoot) {

    document.write('<link rel="stylesheet" type="text/css" href="' + staticFileRoot + '/Content/Uploadify/uploadify.v3.2.css"/>');
    document.write('<script type="text/javascript" src="' + staticFileRoot + '/Content/Uploadify/jquery.uploadify.v3.2.min.js"></script>');

    $.fn.FileUpload = function (param) {

        var uploadparam = {
            'swf': '/Content/Uploadify/uploadify.v3.2.swf',
            'uploader': staticFileRoot + "/FileUpload.ashx",
            'method': 'GET',
            'buttonText' : '选择文件上传',
            'queueID': 'fileQueue',
            'fileSizeLimit': '5MB',
            'fileTypeDesc': "jpg/png/gif Files",
            'fileTypeExts': '*.gif; *.jpg; *.png',
            'auto': true,
            'multi': true,
            'onUploadSuccess': function (file, data, response) {
                eval("data = " + data);

                if (data.err != '') {
                    alert(data.err);
                } else {
                    var msg = data.msg;

                    if (msg.url)
                        msg.absoluteUrl = staticFileRoot + msg.url;

                    if (param.callback != undefined)
                        param.callback(msg,data.err);
                }
            },
            'onUploadError': function (file, errorCode, errorMsg, errorString) {
                alert('The file ' + file.name + ' could not be uploaded: ' + errorString);
            }
        };

        var postParamNames = "subfolder,thumbs,thumbwidth,thumbheight,mode";
        var postParam = {};

        for (var i in param) {
            if (postParamNames.indexOf(i) < 0) {
                uploadparam[i] = param[i];
            }
            else {
                postParam[i] = param[i];
            }
        }

        uploadparam["formData"] = postParam;

        return this.uploadify(uploadparam);
    }

} else {
    alert("请在网站母板页配置全局的'staticFileRoot'");
}

function getThumbAbsoluteUrl(url, suffix) {
    suffix = suffix || "s";
    var ext = url.substring(url.lastIndexOf('.'));
    var head = url.substring(0, url.lastIndexOf('.'));
    url = head + "_" + suffix + ext;
    if (url.indexOf("http") != 0)
        url = staticFileRoot + url;
    
    return url;
}