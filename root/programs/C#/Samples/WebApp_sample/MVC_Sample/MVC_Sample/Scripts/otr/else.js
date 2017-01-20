// 以下のLicenseに従い、このProjectをTemplateとして使用可能です。Release時にCopyright表示してSublicenseして下さい。
// https://github.com/OpenTouryoProject/OpenTouryo/blob/master/license/LicenseForTemplates.txt

function Fx_AdjustStyle() {

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
}