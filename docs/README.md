# Docs

## Setup python virtual env

https://python.land/virtual-environments/virtualenv

```bash
py -m venv py3
```

## Activate virtual env

```ps1
py3\Scripts\Activate.ps1
```

```bash
source py3/bin/activate
```

## Install dependencies

Under project folder:

```bash
pip install -r requirements.txt
```

## Run dev server

```bash
mkdocs serve
```

## Build docs

```bash
mkdocs build
```

## Upgrade Pip packages

```bash
cat requirements.txt | cut -f1 -d= | xargs pip install -U
```
