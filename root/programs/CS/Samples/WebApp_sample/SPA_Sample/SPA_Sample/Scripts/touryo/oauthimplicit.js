//**********************************************************************************
//* Copyright (C) 2017 Hitachi Solutions,Ltd.
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

// -----------------------------------------------------------
// フラグメント（#～の部分）を取得する。
// -----------------------------------------------------------
function getFragment() {
    // URLの「#」記号の後の部分を取得し、
    if (window.location.hash.indexOf("#") === 0) {
        // # が1文字目にある場合
        // 2文字目以降をobjectにparse。
        return parseQueryString(window.location.hash.substr(1));
    } else {
        // そうではない場合。
        return {}; // 空
    }
};

// -----------------------------------------------------------
// QueryStringをobjectにparseする。
// -----------------------------------------------------------
function parseQueryString(queryString) {
    //alert(queryString);
    var data = {},
        pairs, pair, separatorIndex, escapedKey, escapedValue, key, value;

    if (queryString === null) {
        return data; // 空で返す。
    }

    // 分解して、
    pairs = queryString.split("&");

    // 詰めて、
    for (var i = 0; i < pairs.length; i++) {
        pair = pairs[i];
        separatorIndex = pair.indexOf("=");

        if (separatorIndex === -1) {
            escapedKey = pair;
            escapedValue = null;
        } else {
            escapedKey = pair.substr(0, separatorIndex);
            escapedValue = pair.substr(separatorIndex + 1);
        }

        key = decodeURIComponent(escapedKey);
        value = decodeURIComponent(escapedValue);

        // インデクサで。
        data[key] = value;
    }

    // 返す。
    return data;
}

// -----------------------------------------------------------