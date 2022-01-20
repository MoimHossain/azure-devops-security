PAT=$1
commitSha=$BUILD_SOURCEVERSION

echo "Detecting changes...$commitSha"
changedFiles=$(git diff-tree --no-commit-id --name-only -r $commitSha) 
workloads=()
for d in Tribes/*/ ; do  
    if [[ $changedFiles == *"$d"* ]]; then
    workloads+=($d)
    fi
done

for wl in ${workloads[@]}; do
    workloadName="$(basename $wl)"
    echo "<<$workloadName>> changed (full path: $wl)"
    directorypath="${BUILD_SOURCESDIRECTORY}/Tribes/${workloadName}"
    echo "source directory $directorypath"

    docker run --rm -e AzDOAADJoinedPAT="$PAT" -e AzDOAADJoinedURL="https://dev.azure.com/moim/" -v "$directorypath:/home/payload" docker.io/moimhossain/adoctl:v2 apply -d "/home/payload"
done