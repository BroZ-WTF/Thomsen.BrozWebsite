import re
import pathlib
import datetime

ASSEMBLY_FILES = ["*.csproj"]
VERSION_TAGS = ["AssemblyVersion", "FileVersion"]

VERSION_MAJOR = 1
VERSION_MINOR = 0

PROJECT_STARTED_DATE = datetime.date(2025, 4, 30)

days_since_project_started = (datetime.date.today() - PROJECT_STARTED_DATE).days
target_version = f"{VERSION_MAJOR}.{VERSION_MINOR}.{days_since_project_started}"


def find_files(searchpath, files):
    """Finds all project file paths in the given directory"""

    file_paths = []

    for ext in files:
        for filepath in pathlib.Path(searchpath).rglob(f"**/{ext}"):
            file_paths.append(filepath)

    return file_paths


def set_assembly_version(filepath, tags, new_version):
    """Sets the framework version in the given project file"""

    for tag in tags:
        with open(filepath, "r") as file:
            data = file.read()

        match = re.search(f'(?<=\<{tag}\>)[0-9.]+(?=\<\/{tag}\>)', data)

        if match is not None:
            version = match.group(0)

            if version != new_version:
                print(f"{filepath.name}: {tag}: {version} -> {new_version}")

                data = data.replace(
                    f'<{tag}>{version}</{tag}>',
                    f'<{tag}>{new_version}</{tag}>',
                )

                with open(filepath, "w") as file:
                    file.write(data)


if __name__ == "__main__":
    print("searching project paths...")
    projectfile_paths = find_files(".", ASSEMBLY_FILES)

    for path in projectfile_paths:
        set_assembly_version(path, VERSION_TAGS, target_version)
