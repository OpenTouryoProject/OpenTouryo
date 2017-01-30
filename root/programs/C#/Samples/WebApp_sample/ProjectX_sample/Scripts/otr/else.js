//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

// Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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