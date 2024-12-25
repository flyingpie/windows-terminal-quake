# Setup python virtual env
https://python.land/virtual-environments/virtualenv

```shell
py -m venv py3
```

# Activate virtual env

```shell
source py3/bin/activate
```

# Install dependencies

Under project folder:

```shell
pip install -r requirements.txt
```

# Run dev server

```shell
mkdocs serve
```

# Build docs

```shell
mkdocs build
```

# Upgrade Pip packages

```shell
cat requirements.txt | cut -f1 -d= | xargs pip install -U
```