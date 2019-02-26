$(function () {
    nametxt.css('background-image', 'url(' + xinm[0] + ')');
    phonetxt.html(phone[0]);
});

// 开始停止
function start() {

    if (runing) {

        if (pcount <= Lotterynumber) {
            alert("抽奖人数不足");
        } else {
            runing = false;
            $('#start').text('停止');
            startNum()
        }

    } else {
        $('#start').text('自动抽取中(' + Lotterynumber + ')');
        zd();
    }

}

// 开始抽奖

function startLuck() {
    runing = false;
    $('#btntxt').removeClass('start').addClass('stop');
    startNum()
}

// 循环参加名单
function startNum() {
    num = Math.floor(Math.random() * pcount);
    nametxt.css('background-image', 'url(' + xinm[num] + ')');
    phonetxt.html(phone[num]);
    t = setTimeout(startNum, 0);
}

// 停止跳动
function stop() {
    pcount = xinm.length - 1;
    clearInterval(t);
    t = 0;
}

// 打印中奖人

function zd() {
    if (trigger) {

        trigger = false;

        if (pcount >= Lotterynumber) {
            stopTime = window.setInterval(function () {
                if (runing) {
                    runing = false;
                    $('#btntxt').removeClass('start').addClass('stop');
                    startNum();
                } else {
                    runing = true;
                    $('#btntxt').removeClass('stop').addClass('start');
                    stop();

                    Lotterynumber--;

                    $('#start').text('自动抽取中(' + Lotterynumber + ')');

                    if (Lotterynumber===0) {
                        console.log("抽奖结束");
                        window.clearInterval(stopTime);
                        $('#start').text("抽奖结束");
                        $('#start').remove();
                        Lotterynumber--;
                        trigger = true;
                    };

                    //打印中奖者名单
                    $('.luck-user-list').prepend("<li><div class='portrait' style='background-image:url(" + xinm[num] + ")'></div><div class='luckuserName'>" + phone[num] + "</div></li>");
                    $('.modality-list ul').append("<li><div class='luck-img' style='background-image:url(" + xinm[num] + ")'></div><p>" + phone[num] + "</p></li>");
                    //alert('中奖者openid为' + openid[num]);
                    //将已中奖者从数组中"删除",防止二次中奖
                    xinm.splice($.inArray(xinm[num], xinm), 1);
                    phone.splice($.inArray(phone[num], phone), 1);

                    //if ( Lotterynumber == inUser) {
                    //	// 指定中奖人
                    //	nametxt.css('background-image','url(img/7.jpg)');
                    //	phonetxt.html('指定中奖人');
                    //	$('.luck-user-list').prepend("<li><div class='portrait' style='background-image:url(img/7.jpg)'></div><div class='luckuserName'>指定中奖人</div></li>");
                    //	$('.modality-list ul').append("<li><div class='luck-img' style='background-image:url(img/7.jpg)'></div><p>指定中奖人</p></li>");
                    //	inUser = 9999;
                    //}else{
                    //	//打印中奖者名单
                    //	$('.luck-user-list').prepend("<li><div class='portrait' style='background-image:url("+xinm[num]+")'></div><div class='luckuserName'>"+phone[num]+"</div></li>");
                    //	$('.modality-list ul').append("<li><div class='luck-img' style='background-image:url("+xinm[num]+")'></div><p>"+phone[num]+"</p></li>");
                    //	//将已中奖者从数组中"删除",防止二次中奖
                    //	xinm.splice($.inArray(xinm[num], xinm), 1);
                    //	phone.splice($.inArray(phone[num], phone), 1);
                    //};
                }
            }, 1000);
        };
    }
}

