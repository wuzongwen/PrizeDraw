layui.use(['form', 'layer', 'jquery', 'admin'], function () {
    var form = layui.form,
        admin = layui.admin,
        layer = layui.layer,
        $ = layui.jquery;

    //失去焦点时判断值为空不验证，一旦填写必须验证
    $('input[name="Email"]').blur(function () {
        //这里是失去焦点时的事件
        if ($('input[name="Email"]').val()) {
            $('input[name="Email"]').attr('lay-verify', 'email');
        } else {
            $('input[name="Email"]').removeAttr('lay-verify');
        }
    });

    //非必填项
    if (window.location.pathname !== "/User/EditPwd") {
        $('input[name="Password"]').blur(function () {
            //这里是失去焦点时的事件
            if ($('input[name="Password"]').val()) {
                $('input[name="Password"]').attr('lay-verify', 'Password');
                $('input[name="RePassword"]').attr('lay-verify', 'RePassword');
            } else {
                $('input[name="Password"]').removeAttr('lay-verify');
                $('input[name="RePassword"]').removeAttr('lay-verify');
            }
        });
    }

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

    //修改用户
    form.on('submit(edit)', function (data) {
        ////发异步，把数据提交至后台
        $.ajax({
            type: 'post',
            url: '/User/UserEdit',
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

    //修改用户
    form.on('submit(editpwd)', function (data) {
        ////发异步，把数据提交至后台
        $.ajax({
            type: 'post',
            url: '/User/PwdEdit',
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
                            //跳转至用户注销页
                            top.location.href = "/Admin/LoginOut";
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