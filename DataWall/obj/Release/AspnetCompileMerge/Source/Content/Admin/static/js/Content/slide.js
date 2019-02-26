layui.use(['admin', 'form', 'jquery', 'layer', 'upload'], function () {
    var form = layui.form,
        $ = layui.jquery,
        admin = layui.admin,
        upload = layui.upload,
        layer = layui.layer;

    //图片上传
    upload.render({
        elem: '#FileAdd'
        , url: '/Content/Upload'
        , multiple: true
        , before: function (obj) {
            //加载层-风格4
            layer.msg('图片上传中', {
                icon: 16
                , shade: 0.01
            });
            //预读本地文件示例，不支持ie8
            obj.preview(function (index, file, result) {
                $('#ImageList').removeClass('layui-hide');
            });
        }
        , done: function (res) {
            layer.closeAll("loading");
            //上传完毕
            $('#FilePath').val(res.src);
            $('#demo2').html('<img style="max-width:100px;margin-right:5px" src="/' + res.thumbsrc + '" alt="' + res.fileName + '" class="layui-upload-img">');
        }
        , error: function () {
            //请求异常回调
            layer.msg('网络繁忙，请稍后重试...');
            layer.closeAll("loading");
            return false;
            //关闭
        }
        , accept: 'images'
        , size: 10240
        , number: 1
    });

    //自定义验证规则
    form.verify({
        UploadImg: function (value) {
            if (value.length === 0) {
                return '请上传图片';
            }
        }
    });


    //添加奖项
    form.on('submit(add)', function (data) {
        //发异步，把数据提交至后台
        $.ajax({
            type: 'post',
            url: '/Content/SlideAdd',
            dataType: 'json',
            data: $('.layui-form').serialize(),
            beforeSend: function () {
                //加载层-风格4
                layer.msg('数据提交中', {
                    icon: 16
                    , shade: 0.01
                    , time: 60 * 1000
                });
            },
            complete: function () {
                layer.closeAll("loading");
            },
            error: function (XMLHttpRequest, status, thrownError) {
                layer.msg('网络繁忙，请稍后重试...');
                return false;
            },
            success: function (msg) {
                var code = msg.code;
                switch (code) {
                    case '200':
                        layer.alert(msg.msg, { icon: 6 }, function () {
                            // 获得frame索引
                            var index = parent.layer.getFrameIndex(window.name);
                            //关闭当前frame
                            parent.layer.close(index);
                            //刷新父页面
                            parent.location.reload();
                        });
                        break;
                    case '201':
                        layer.msg(msg.msg);
                        break;
                    case '202':
                        layer.msg(msg.msg);
                        break;
                    case '401':
                        layer.msg(msg.msg, {
                            icon: 5,
                            time: 1000
                        });
                        break;
                    case '402':
                        layer.alert(msg.msg, { icon: 5 }, function () {
                            //跳转到登录页
                            top.location.href = msg.url;
                        });
                        break;
                }
                return false;
            }
        });
        return false;
    });

    //修改奖项
    form.on('submit(edit)', function (data) {
        //发异步，把数据提交至后台
        $.ajax({
            type: 'post',
            url: '/Content/SlideEdit',
            dataType: 'json',
            data: $('.layui-form').serialize(),
            beforeSend: function () {
                //加载层-风格4
                layer.msg('数据提交中', {
                    icon: 16
                    , shade: 0.01
                    , time: 60 * 1000
                });
            },
            complete: function () {
                layer.closeAll("loading");
            },
            error: function (XMLHttpRequest, status, thrownError) {
                layer.msg('网络繁忙，请稍后重试...');
                return false;
            },
            success: function (msg) {
                var code = msg.code;
                switch (code) {
                    case '200':
                        layer.alert(msg.msg, { icon: 6 }, function () {
                            // 获得frame索引
                            var index = parent.layer.getFrameIndex(window.name);
                            //关闭当前frame
                            parent.layer.close(index);
                            //刷新父页面
                            parent.location.reload();
                        });
                        break;
                    case '201':
                        layer.msg(msg.msg);
                        break;
                    case '202':
                        layer.msg(msg.msg);
                        break;
                    case '401':
                        layer.msg(msg.msg, {
                            icon: 5,
                            time: 1000
                        });
                        break;
                    case '402':
                        layer.alert(msg.msg, { icon: 5 }, function () {
                            //跳转到登录页
                            top.location.href = msg.url;
                        });
                        break;
                }
                return false;
            }
        });
        return false;
    });

    //select赋值
    $(document).ready(function () {
        $('#Library').val($('#LibraryId').val());
        form.render('select'); //刷新select选择框渲染
    });
});