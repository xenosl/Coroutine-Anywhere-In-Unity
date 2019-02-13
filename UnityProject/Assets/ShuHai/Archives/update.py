from zipfile import ZipFile
import os
import sys

# region Archive Path Mapping

CSHARP_PROJECT_DIR = "../../../../../CSharpProject"
ASSEMBLIES_PATH_IN_ARCHIVE = "ShuHai/Assemblies"

ArchivePathMapping = {}


def initialize_archive_path_mapping(version):
    mapping = {}
    add_archive_path_mapping(mapping,
        CSHARP_PROJECT_DIR + "/ShuHai.Unity.Coroutines/bin/Release-" + version,
        ASSEMBLIES_PATH_IN_ARCHIVE, "ShuHai.Unity.Coroutines.dll")
    add_archive_path_mapping(mapping,
        CSHARP_PROJECT_DIR + "/ShuHai.Unity.Coroutines/bin/Release-" + version,
        ASSEMBLIES_PATH_IN_ARCHIVE, "ShuHai.Unity.Coroutines.pdb")
    add_archive_path_mapping(mapping,
        CSHARP_PROJECT_DIR + "/ShuHai.Unity.Coroutines.Editor/bin/Release",
        ASSEMBLIES_PATH_IN_ARCHIVE, "ShuHai.Unity.Coroutines.Editor.dll")
    add_archive_path_mapping(mapping,
        CSHARP_PROJECT_DIR + "/ShuHai.Unity.Coroutines.Editor/bin/Release",
        ASSEMBLIES_PATH_IN_ARCHIVE, "ShuHai.Unity.Coroutines.Editor.pdb")
    ArchivePathMapping[version] = mapping


def add_archive_path_mapping(mapping, diskDir, archiveDir, fileName):
    if diskDir.startswith(".."):
        diskDir = os.path.abspath(os.path.join(__file__, diskDir))
    diskPath = os.path.join(diskDir, fileName)
    archivePath = os.path.normpath(os.path.join(archiveDir, fileName))
    mapping[diskPath] = archivePath

# endregion Archive Path Mapping


def create_archive(version):
    zipPath = os.path.join(os.path.dirname(__file__), version + ".zip")
    print("Creating archive: " + zipPath)
    if os.path.isfile(zipPath):
        os.remove(zipPath)
    with ZipFile(zipPath, 'w') as zip:
        for diskPath, archivePath in ArchivePathMapping[version].items():
            zip.write(diskPath, archivePath)


def main():
    versions = ["5.3+", "2017.1+", "2018.3+"]
    try:
        for ver in versions:
            initialize_archive_path_mapping(ver)
            create_archive(ver)
        print("Done!")
    except:
        print(sys.exc_info()[1])


if __name__ == "__main__":
    main()
