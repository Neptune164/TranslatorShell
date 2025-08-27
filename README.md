# TranslatorShell

## Overview
TranslatorShell is a modular real-time translation system for Final Fantasy XIV, consisting of a Dalamud plugin and a local FastAPI service, designed to capture NPC dialogues, process them through open-source/AI 
translation engines, and display bilingual output seamlessly within the game.

## Prerequisites
- .NET SDK 8.0+ (windows)
- Visual Studio 2022 (recommended)
- Dalamud

## How to run
- Clone the repository
- Open TranslatorShell.sln using VS 2022
- Build Solution (Debug or Release both work)
- Launch the game using XIVLauncher and load the "TranslatorShell.dll".

## Future work
- More selections on translation APIs
- More settings on UI and the overlay
- Handle the translation delay
