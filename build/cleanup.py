#!/usr/bin/env python3

import os
import shutil


def find_artifact_paths(path):
    artifact_paths = []

    for root, dirs, files in os.walk(path):
        for sub in ["bin", "obj", "publish"]:
            if sub in dirs:
                artifact_paths.append(os.path.join(root, sub))

    return artifact_paths


if __name__ == "__main__":
    print("searching project paths...")
    artifact_paths = find_artifact_paths(".")

    for path in artifact_paths:
        print(f"... deleting artifacts in {path}")

        shutil.rmtree(path)
                