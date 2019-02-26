layui.use(['laydate', 'jquery', 'admin'], function () {
    var laydate = layui.laydate,
        $ = layui.jquery,
        admin = layui.admin;
    //执行一个laydate实例
    laydate.render({
        elem: '#start' //指定元素
    });
    //执行一个laydate实例
    laydate.render({
        elem: '#end' //指定元素
    });

    /*场馆-删除*/
    window.member_del = function (obj, id, lid) {
        layer.confirm('确认要删除吗？', function (index) {
            //发异步删除数据
            $.ajax({
                type: 'post',
                url: '/Library/DelLibraryUser',
                dataType: 'json',
                data: {
                    id: id,
                    LibraryId: lid
                },
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
                                //刷新当前iframe
                                window.location.reload(index);
                            });
                            break;
                        case '201':
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
        });
    }

    /*场馆-删除*/
    window.member_add = function (obj, id, lid) {
        layer.confirm('确认要添加吗？', function (index) {
            //发异步删除数据
            $.ajax({
                type: 'post',
                url: '/Library/AddLibraryUser',
                dataType: 'json',
                data: {
                    id: id,
                    LibraryId: lid
                },
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
                                //刷新当前iframe
                                window.location.reload(index);
                            });
                            break;
                        case '201':
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
        });
    }
});