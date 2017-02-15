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

function Fx_CommonAdjustStyle(offset) {
    var menuTopMargin = $('.page-header').height();
    var targetScrollValue = offset.top - menuTopMargin;

    var wScrollvalue = $(window).scrollTop();
    var span_test = $("span.test");
    if (span_test) { span_test.text(wScrollvalue); }

    var menuZIndex = $(".nav-side-menu").css('z-index');
    if (menuZIndex === 'auto') {
        // サイドバーの z-index が auto の場合は、画面の横幅が広い (PC 向け)

        if (wScrollvalue > targetScrollValue) {
            if (span_test) { span_test.append(" / " + targetScrollValue + " / " + offset.top); }

            // サイドバーの位置を調整する
            var movePosition = wScrollvalue + "px";
            $(".nav-side-menu").css('top', movePosition);
        }
        else {
            $(".nav-side-menu").css('top', 0);
        }
    }
    else {
        // サイドバーの z-index が auto でない場合は、画面の横幅が狭い (スマホ・タブレット向け)
        $(".nav-side-menu").css('top', menuTopMargin);
    }
}

function Fx_AdjustStyle() {
    // 画面初期化時の、サイドバーのオフセットを退避しておく
    offset = $(".nav-side-menu").offset();

    // 画面表示時に、描画位置を調整する
    Fx_CommonAdjustStyle(offset);

    // スクロール時に、サイドバーの位置を調整する
    $(window).scroll(function () {
        Fx_CommonAdjustStyle(offset);
    });
    // ウィンドウのリサイズ時に、描画位置を調整する
    $(window).resize(function () {
        var scrollTop = $(window).scrollTop();
        Fx_CommonAdjustStyle(offset);
    });
}
