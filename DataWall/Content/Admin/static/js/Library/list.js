layui.use(['laydate', 'jquery', 'admin'], function () {
    var laydate = layui.laydate,
        $ = layui.jquery,
        admin = layui.admin;

    //加载结束
    $(document).ready(function () {
        parent.layer.closeAll('loading');
    });

    //执行一个laydate实例
    laydate.render({
        elem: '#start' //指定元素
    });
    //执行一个laydate实例
    laydate.render({
        elem: '#end' //指定元素
    });
    /*场馆-停用*/
    window.member_stop = function (obj, id) {
        var title, enable;
        if ($(obj).attr('title') === '启用') {
            title = "启用";
            enable = 0;
        }
        else {
            title = "停用";
            enable = 1;
        }
        layer.confirm('确认要' + title + '吗？', function (index) {
            //发异步把用户状态进行更改
            $.ajax({
                type: 'post',
                url: '/Library/EditEnable',
                dataType: 'json',
                data: {
                    id: id,
                    enable: enable
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
                            if (enable === 1) {
                                $(obj).attr('title', '启用')
                                $(obj).find('i').html('✔');
                                $(obj).parents("tr").find(".td-status").find('span').addClass('layui-btn-disabled').html('已' + title);
                                layer.msg('已' + title, {
                                    icon: 5,
                                    time: 1000
                                });
                            }
                            else {
                                $(obj).attr('title', '停用')
                                $(obj).find('i').html('✘');
                                $(obj).parents("tr").find(".td-status").find('span').removeClass('layui-btn-disabled').html('已' + title);
                                layer.msg('已' + title, {
                                    icon: 6,
                                    time: 1000
                                });
                            }
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
    window.member_del = function (obj, id, page) {
        layer.confirm('确认要删除吗？', function (index) {
            //发异步删除数据
            $.ajax({
                type: 'post',
                url: '/Library/DelLibrary',
                dataType: 'json',
                data: {
                    id: id,
                    page: page
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
                            layer.alert(msg.msg, { icon: 1 }, function () {
                                location.href = "/Library/List?page=" + msg.page;
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

    window.delAll = function (page) {
        var data = tableCheck.getData();
        if (data.length > 0) {
            layer.confirm('确认要删除吗？', function (index) {
                //发异步删除数据
                $.ajax({
                    type: 'post',
                    url: '/Library/DelLibraryAll',
                    dataType: 'json',
                    data: {
                        idList: String(data),
                        page: page
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
                                layer.alert(msg.msg, { icon: 1 }, function () {
                                    location.href = "/Library/List?page=" + msg.page;
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
        else {
            layer.msg('请选择需要删除的场馆', {
                icon: 5,
                time: 1000
            });
        }
    }
});