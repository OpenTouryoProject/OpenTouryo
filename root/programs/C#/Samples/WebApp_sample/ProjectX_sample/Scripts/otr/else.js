// 以下のLicenseに従い、このProjectをTemplateとして使用可能です。Release時にCopyright表示してSublicenseして下さい。
// https://github.com/OpenTouryoProject/OpenTouryo/blob/master/license/LicenseForTemplates.txt

function Fx_AdjustStyle() {

    // 左menuのサイズを調整
    var sidemenu = $("#sidemenu");
    var contents = $("#contents");

    if (contents.height() >= sidemenu.height()) {
        sidemenu.height(contents.height());
    }
    else {
        contents.height(sidemenu.height());
    }

    var browserHeight = Fx_getBrowserHeight()
    if (sidemenu.height() < browserHeight) {
        sidemenu.height(browserHeight);
    }

    // 左menuをスクロールに合わせて移動
    var memuPosi = $("#sidemenucontent").offset();
    var menuTopMargin = 105;
    var targetScrollValue = memuPosi.top - menuTopMargin;

    $(window).scroll(function () {
        var wScrollvalue = $(window).scrollTop();
        var span_test = $("span.test");
        if (span_test) { span_test.text(wScrollvalue); }
        if (wScrollvalue > targetScrollValue) {
            if (span_test) { span_test.append(" / " + targetScrollValue + " / " + memuPosi.top); }
            $("#sidemenucontent").css({
                position: "fixed",
                top: menuTopMargin
            });
        }
        else {
            $("#sidemenucontent").css({
                position: "absolute",
                top: menuTopMargin
            });
        }
    });
}