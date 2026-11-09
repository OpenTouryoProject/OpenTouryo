rem z_GrepResult1.bat and 2.bat must be Shift_JIS because they pass Japanese to sakura. This one is ASCII.
set PATH=C:\Program Files (x86)\sakura;%PATH%
sakura -GREPMODE -GKEY="warning" -GFOLDER="." -GFILE="*.log" -GOPT=P -GCODE=4
