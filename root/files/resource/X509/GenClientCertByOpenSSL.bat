@echo off
rem NOTE: keep this file pure ASCII. cmd.exe reads a batch file by byte offset, so non-ASCII text
rem       can break lines depending on the console code page (root/CODING.md section 4. #342).
rem
rem Recreate the client certificate (for mTLS).
rem
rem   - Output: _SHA256RSAClientCert.cer / _SHA256RSAClientCert.pfx
rem     (prefixed with "_" so that the existing files are not overwritten; check, then rename to replace)
rem   - Password: test (same as SpRp_ClientCertPfxPassword in the configuration file)
rem   - The subject must match tls_client_auth_subject_dn of the client registration
rem     (in the template, the value of FAPI1_Client / FAPI2_Client)
rem   - PKCS#12 uses the legacy encryption, so that net48 (.NET Framework) can read it
rem
rem Details: root/CONFIGURATION.md section 8.

where openssl >nul 2>&1
if errorlevel 1 (
    echo openssl is not on PATH.
    rem Do not write parentheses in this block: a closing one would end the "if" block.
    echo Install OpenSSL, or add mingw64\bin of Git for Windows to PATH, then run again.
    exit /b 1
)

set SUBJECT=/CN=MPAS Test Client
set DAYS=36500
set PASSWORD=test

openssl req -x509 -newkey rsa:2048 -sha256 -days %DAYS% -nodes ^
  -keyout _client-private-key.pem -out _SHA256RSAClientCert.cer ^
  -subj "%SUBJECT%" -addext "extendedKeyUsage=clientAuth" -addext "basicConstraints=CA:FALSE"

openssl pkcs12 -export -inkey _client-private-key.pem -in _SHA256RSAClientCert.cer ^
  -out _SHA256RSAClientCert.pfx -passout pass:%PASSWORD% ^
  -keypbe PBE-SHA1-3DES -certpbe PBE-SHA1-3DES -macalg sha1

del _client-private-key.pem

openssl x509 -in _SHA256RSAClientCert.cer -noout -subject -dates
