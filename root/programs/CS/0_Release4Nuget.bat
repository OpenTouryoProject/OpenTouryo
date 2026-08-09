@echo off
@rem --------------------------------------------------
@rem Build for the NuGet packages (#531)
@rem
@rem Pass DEBUG_TYPE=portable. z_Common.bat honours the value when the
@rem caller has already set it. z_Common.bat used to be edited by hand
@rem and reverted afterwards, which risked publishing with "full" when
@rem the revert was forgotten.
@rem
@rem   - a .snupkg is only accepted when the PDB is portable
@rem   - the Source Link information also lives in the portable PDB
@rem
@rem NOTE: keep this file pure ASCII (#532).
@rem --------------------------------------------------
set DEBUG_TYPE=portable

@rem --------------------------------------------------
@rem Normalize the source paths recorded in the PDB to /_/... (#531).
@rem
@rem   - the published package no longer carries the local paths of the
@rem     machine that built it
@rem   - Visual Studio opens the file at the path in the PDB when it exists,
@rem     so an absolute path means Source Link is never used on the build
@rem     machine. Normalizing lets it be verified there as well.
@rem
@rem z_Common.bat defaults this to false, so ordinary builds are unaffected.
@rem --------------------------------------------------
set CI_BUILD=true

@echo on
timeout 5

echo | call 1_DeleteDir.bat
echo | call 2_Build_NuGet_net48.bat

@echo on
timeout 5

echo | call 1_DeleteDir.bat
echo | call 2_Build_NuGet_netcore100.bat

@echo on
timeout 5

echo | call 4_Build_CopyAssemblies.bat