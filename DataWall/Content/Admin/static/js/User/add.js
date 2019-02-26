layui.use(['form', 'jquery', 'admin', 'layer'], function () {
    var form = layui.form,
        $ = layui.jquery,
        admin = layui.admin,
        layer = layui.layer;

    //失去焦点时判断值为空不验证，一旦填写必须验证
    $('input[name="Email"]').blur(function () {
        //这里是失去焦点时的事件
        if ($('input[name="Email"]').val()) {
            $('input[name="Email"]').attr('lay-verify', 'email');
        } else {
            $('input[name="Email"]').removeAttr('lay-verify');
        }
    });

    //自定义验证规则
    form.verify({
        UserName: function (value) {
            if (value.length < 5) {
                return '昵称至少得5个字符';
            }
        },
        Password: function (value) {
            if (value.length < 6) {
                return '密码至少6位';
            }
        },
        RePassword: function (value) {
            if (value !== $('#Password').val()) {
                return '两次密码不一致';
            }
        }
    });
   

    //监听提交
    form.on('submit(add)', function (data) {
        ////发异步，把数据提交至后台
        $.ajax({
            type: 'post',
            url: '/User/UserAdd',
            dataType: 'json',
            data: $('.user-form').serialize(),
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

});