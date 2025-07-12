#!/bin/bash

set -e # Quit on error.

declare -A COMMANDS # We use this to build a list of available commands with descriptions.

# -----------------------
# Commands
# -----------------------

base() {
	export COMPOSE_PROFILES=data,infra,misc,otel

	docker compose up --build -d
}
COMMANDS[base]="Ups everything that is not DTR-specific (infra, databases, etc.)."

lite() {
	export COMPOSE_PROFILES=data,infra,misc,otel,dtrlite

	docker compose up --build -d
}
COMMANDS[lite]="Run DTR - Light version."

# full() {
# 	export COMPOSE_PROFILES=data,infra,misc,otel,dtrfull
#
# 	docker compose up --build -d
# }
# COMMANDS[full]="Run DTR - Closer-to-production version."

lite-dotnet() {
	export COMPOSE_PROFILES=data,infra,misc,otel,dtrlite-dotnet

	docker compose up --build -d
}
COMMANDS[lite-dotnet]="Run DTR - Light version (.Net stuff only)."

lite-java() {
	export COMPOSE_PROFILES=data,infra,misc,otel,dtrlite-java

	docker compose up --build -d
}
COMMANDS[lite-java]="Run DTR - Light version (Java stuff only)."

down() {
	docker compose --profile "*" down
}
COMMANDS[down]="Turns everything off."

nuke() {
	# Check for existence of the data-directory
	echo "!!WARNING!!"
	echo ""
	echo "This will delete everything DTR related, including data."
	echo ""
	echo "!!WARNING!!"
	echo ""
	read -p "Are you sure? [y/N] " -n 1 -r
	echo

	if ! [[ $REPLY =~ [Yy]$ ]]; then
		echo "EXIT, no harm done"
		exit 1
	fi

	echo "            Nuking DTR...         "
	echo "⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀"⠀
	echo "⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣤⡖⠛⠋⠉⠉⠉⠑⠒⠦⠤⠶⠒⠒⠦⣀⠀⠀⠀⠀⠀⠀⠀"⠀
	echo "⠀⠀⠀⠀⠀⠀⢀⣴⣞⡿⠻⠶⠂⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⠶⠦⣄⡀⠀⠀⠀"⠀
	echo "⠀⠀⠀⠀⡴⠶⠿⠻⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠱⣼⣦⠀⠀"⠀
	echo "⠀⠀⠀⡾⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢽⣦⠀"⠀
	echo "⠀⠀⣾⢻⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⡄"⠀
	echo "⠀⠀⠈⠳⣽⠂⡀⠀⠀⠀⠀⠀⠀⢰⣦⣼⣿⣦⣾⣷⣶⣦⣴⣦⡀⠀⠀⠀⠀⠀⠀⠀⢀⡇"⠀
	echo "⠀⠀⠀⠀⠙⠒⢤⣴⣶⣶⣶⣤⣶⣾⣿⠏⠉⢻⠀⠹⠁⣿⣿⣿⣧⠀⠀⠀⣠⣴⣶⡶⠋⠀"⠀
	echo "⠀⠀⠀⠀⠀⠀⠀⠈⠉⠛⠛⠻⠿⣿⣿⡅⠀⠈⠀⠀⠀⣿⣿⣿⣿⣿⣷⡶⠛⠛⠉⠀⠀⠀"⠀
	echo "⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣘⠇⠀⠀⠀⠀⠀⡟⠉⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀"⠀
	echo "⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⢐⣨⣇⠀⢰⢀⢳⣆⣷⣦⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀"⠀
	echo "⠀⠀⠀⠀⠀⠀⠀⠀⣀⡤⠒⠉⠉⠉⠀⠈⠓⠛⠚⠋⠁⠀⠀⠉⠉⠲⣦⣄⠀⠀⠀⠀⠀⠀"⠀
	echo "⠀⠀⠀⠀⠀⠀⠀⠸⣿⣄⠀⠀⠀⢀⡀⠀⣀⣀⣀⣀⣀⣀⠀⠀⠀⣴⣿⠿⠀⠀⠀⠀⠀⠀"⠀
	echo "⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠛⠻⢿⠿⢿⠋⣿⢿⠛⠛⢿⣿⠿⠟⠛⠛⠁⠀⠀⠀⠀⠀⠀⠀"⠀
	echo "⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢈⡄⠀⣸⠀⣿⢸⠀⠀⠘⣧⠤⠀⠀⠀⠄⠀⠀⠀⠀⠀⠀⠀"⠀
	echo "⠀⠀⠀⠀⠀⠀⠀⠀⠀⢤⠄⠈⠀⣶⣿⠀⣸⠀⠀⠇⠀⣿⣿⡂⠐⠺⠀⠀⠀⠀⠀⠀⠀⠀"⠀
	echo "⠀⠀⠀⠀⢀⣀⣴⣿⣶⣶⣄⣰⠏⢉⣿⠃⠀⠄⠀⠀⠀⢹⣇⠈⢶⣤⣄⡀⠀⠀⠀⠀⠀⠀"⠀
	echo "⠀⠀⣀⣴⣿⣿⣿⣿⣿⣿⣿⣿⣤⣿⣯⡀⠀⣷⠶⣴⣀⡀⣻⣷⣋⣹⣿⣿⣿⣦⣄⡀⠀⠀"⠀
	echo "⠒⠋⠁⠀⠁⠀⠙⠻⠿⢻⣿⣿⣿⣿⣷⣭⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⢿⣿⣿⣿⡻⣿⣶⣤"⡄
	echo "⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠉⠉⠉⠀⠙⠛⠛⠛⠛⠛⠉⠉⠉⠀⠀⠀⠉⠉⠁⠙⠙⠚"⠀

	PREFIX=dtr

	# Containers
	for ID in $(docker ps -a --filter "name=^$PREFIX" -q); do
		docker rm -f "$ID"
	done

	# Volumes
	for ID in $(docker volume ls --filter "name=^$PREFIX" -q); do
		docker volume rm -f "$ID"
	done

	# Networks
	for ID in $(docker network ls --filter "name=^$PREFIX" -q); do
		docker network rm -f "$ID"
	done

	echo "Done!"
}
COMMANDS[nuke]="Remove everything DTR-related."

# -----------------------
# Show Help
# -----------------------

show_help() {
	echo "Usage: $0 <command>"
	echo ""
	echo "Available commands:"
	for CMD in "${!COMMANDS[@]}"; do
		printf "  %-15s %s\n" "$CMD" "${COMMANDS[$CMD]}"
	done
}

# -----------------------
# Command Dispatch
# -----------------------

if [ $# -eq 0 ]; then
	show_help
	exit 0
fi

COMMAND="$1"
shift

if declare -F "$COMMAND" >/dev/null; then
	"$COMMAND" "$@"
else
	echo "Unknown command: $COMMAND"
	echo ""
	show_help
	exit 1
fi
