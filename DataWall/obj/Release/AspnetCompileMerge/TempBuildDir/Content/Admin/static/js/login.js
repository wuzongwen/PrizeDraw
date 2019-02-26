layui.use(['form', 'jquery', 'admin'], function () {
    var form = layui.form
        ,admin = layui.admin;
    var $ = layui.jquery;
    form.on('submit(login)', function (data) {
        $.ajax({
            type: 'post',
            url: '/Admin/LoginValidate',
            dataType: 'json',
            data: {
                UserName: data.field['UserName'],
                Password: data.field['Password']
            },
            beforeSend: function () {
              //加载层-风格4
                layer.msg('登录中..', {
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
                        layer.msg("登录成功");
                        setTimeout(function () {
                            location.href = "/Admin/Index";
                        }, 2000);
                        break;
                    case '201':
                        layer.msg(msg.msg);
                        $('.loginin').val("登录");
                        break;
                    case '202':
                        layer.msg(msg.msg);
                        $('.loginin').val("登录");
                        break;
                }
                return false;
            }
        });
        return false;
    });
});