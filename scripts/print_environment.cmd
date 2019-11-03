@setlocal
@echo off

echo APPVEYOR_BUILD_NUMBER:                  %APPVEYOR_BUILD_NUMBER%
echo APPVEYOR_BUILD_VERSION:                 %APPVEYOR_BUILD_VERSION%
echo APPVEYOR_PULL_REQUEST_NUMBER:           %APPVEYOR_PULL_REQUEST_NUMBER%
echo APPVEYOR_PULL_REQUEST_TITLE:            %APPVEYOR_PULL_REQUEST_TITLE%
echo APPVEYOR_PULL_REQUEST_HEAD_REPO_NAME:   %APPVEYOR_PULL_REQUEST_HEAD_REPO_NAME%
echo APPVEYOR_PULL_REQUEST_HEAD_REPO_BRANCH: %APPVEYOR_PULL_REQUEST_HEAD_REPO_BRANCH%
echo APPVEYOR_PULL_REQUEST_HEAD_COMMIT:      %APPVEYOR_PULL_REQUEST_HEAD_COMMIT%
echo APPVEYOR_REPO_NAME:                     %APPVEYOR_REPO_NAME%
echo APPVEYOR_REPO_BRANCH:                   %APPVEYOR_REPO_BRANCH%
echo APPVEYOR_REPO_TAG:                      %APPVEYOR_REPO_TAG%
echo APPVEYOR_REPO_COMMIT:                   %APPVEYOR_REPO_COMMIT%
echo APPVEYOR_REPO_COMMIT_AUTHOR:            %APPVEYOR_REPO_COMMIT_AUTHOR%
echo APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL:      %APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL%
echo APPVEYOR_REPO_COMMIT_MESSAGE:           %APPVEYOR_REPO_COMMIT_MESSAGE%
echo APPVEYOR_SCHEDULED_BUILD:               %APPVEYOR_SCHEDULED_BUILD%
echo APPVEYOR_FORCED_BUILD:                  %APPVEYOR_FORCED_BUILD%
echo APPVEYOR_RE_BUILD:                      %APPVEYOR_RE_BUILD%

endlocal