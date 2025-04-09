# Memory Game
A comprehensive Memory Card Game application developed with C# and WPF using the MVVM architectural pattern.

## Description
Memory Game is an interactive card-matching game where players test their memory by finding pairs of matching cards. The game features multiple difficulty levels, customizable board sizes, and different image categories to keep the gameplay experience fresh and engaging.

## Features

- **User Management System**
  - Create and manage multiple user profiles
  - Select avatars for personalization
  - Track game statistics (games played, won, win rate)

- **Customizable Game Settings**
  - Standard mode (4x4 board)
  - Custom mode with configurable dimensions
  - Three different image categories (Animals, Food, Landscapes)
  - Adjustable time limits

- **Game Functionality**
  - Interactive card flipping and matching
  - Timer countdown
  - Visual feedback for matches
  - Game state tracking

- **Save/Load System**
  - Save games in progress
  - Resume saved games later
  - Automatic cleanup of completed games

- **Statistics Dashboard**
  - View performance metrics for all players
  - Compare win rates and games played

## Technical Implementation
The application follows the MVVM (Model-View-ViewModel) architecture for clean separation of concerns:
- **Models**: Represent the core data structures (Card, User, GameState)
- **ViewModels**: Handle the game logic and state management
- **Views**: Define the user interface with XAML
- **Services**: Provide utilities for data persistence and messaging

Additional technical highlights:
- Data binding for automatic UI updates
- Command pattern for handling user interactions
- Publisher/Subscriber messaging system for component communication
- Custom value converters for data transformation
- JSON serialization for game state persistence

## How to Play
1. Select or create a user profile
2. Configure your game settings
3. Start a new game
4. Click on cards to flip them
5. Find all matching pairs before time runs out
