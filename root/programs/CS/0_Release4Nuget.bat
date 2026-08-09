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