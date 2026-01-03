# 🗡️ Dark Legend

A 3D RPG Game inspired by MU Online - Built with Unity

## Features
- ⚔️ 3 Character Classes: Dark Knight, Dark Wizard, Elf (Coming Soon)
- 📊 Stats System: STR, AGI, VIT, ENE (Coming Soon)
- 🎯 Skill System with cooldowns (Coming Soon)
- 👹 Monster AI & Combat (Coming Soon)
- 🎒 Inventory & Equipment System (Coming Soon)
- 🖥️ PC Controls (Keyboard + Mouse)
- 📱 **Android Mobile Support**

## Tech Stack
- Unity 2022.3 LTS
- C#
- Android SDK (API 24+)

## 📱 Mobile Support

Dark Legend now includes comprehensive Android mobile support with:

### Touch Controls
- **Virtual Joystick** - Smooth touch-based movement control
- **Touch Buttons** - Attack, Skills, Dodge, and Interact buttons
- **Gesture Support** - Swipe, pinch, and tap detection
- **Camera Control** - Touch rotation and pinch-to-zoom

### Mobile UI
- **Optimized HUD** - Health, Mana, and Experience bars
- **Skill Bar** - Wheel or Grid style skill layouts
- **Mobile Inventory** - Touch-friendly inventory interface
- **Quick Slots** - Fast access to potions and items
- **Auto-Play System** - Automated combat and movement

### Performance Optimization
- **Quality Settings** - Adaptive quality based on device
- **LOD Management** - Level of Detail optimization
- **Frame Rate Control** - Adaptive FPS for smooth gameplay
- **Battery Saver** - Reduced power consumption mode
- **Memory Management** - Automatic cleanup and optimization

### Platform Features
- **Haptic Feedback** - Vibration feedback for actions
- **Notifications** - Local push notifications
- **Safe Area Support** - Notch and rounded corner handling
- **Screen Management** - Orientation and brightness control

## 📱 Mobile Build Guide

### Requirements
- Unity 2022.3 LTS
- Android SDK (API 24+)
- JDK 11
- IL2CPP scripting backend support

### Build Steps

1. **Open Project in Unity**
   ```
   File > Open Project
   ```

2. **Switch Platform to Android**
   ```
   File > Build Settings > Android > Switch Platform
   ```

3. **Configure Player Settings**
   ```
   Edit > Project Settings > Player
   ```
   - **Product Name**: Dark Legend
   - **Company Name**: Dark Legend Studio
   - **Package Name**: com.darklegend.game
   - **Version**: 1.0.0
   - **Minimum API Level**: Android 7.0 (API 24)
   - **Target API Level**: Android 13 (API 33)
   - **Scripting Backend**: IL2CPP
   - **Target Architectures**: ARM64

4. **Configure Quality Settings**
   ```
   Edit > Project Settings > Quality
   ```
   - Set up quality levels for mobile devices
   - Configure shadow and texture quality

5. **Build APK**
   ```
   File > Build Settings > Build
   ```
   Or use **Build and Run** to test on connected device

### Mobile Controls

#### Touch Controls
- **Left Side**: Virtual Joystick for movement
- **Right Side**: Skill buttons (wheel or grid layout)
- **Touch & Drag**: Rotate camera
- **Pinch**: Zoom in/out
- **Auto Button**: Enable/disable auto-play mode

#### Keyboard Controls (PC/Editor)
- **WASD**: Movement
- **Mouse**: Camera control
- **Space**: Attack
- **1-4**: Skills
- **E**: Interact
- **Shift**: Dodge

### Architecture

```
Assets/Scripts/Mobile/
├── Core/              # Core mobile systems
├── Input/             # Touch input and controls
├── Camera/            # Mobile camera controls
├── UI/                # Mobile UI components
├── Performance/       # Optimization systems
└── Platform/          # Platform-specific features
```

### Key Components

- **MobileManager** - Central manager for all mobile systems
- **TouchInputManager** - Handles all touch input
- **VirtualJoystick** - Touch-based movement control
- **MobileCameraController** - Camera with touch controls
- **MobileUIManager** - Manages mobile UI elements
- **AutoPlaySystem** - Automated gameplay system

## Development

### Testing Mobile Features in Editor
Most mobile features can be tested in the Unity Editor using mouse input:
- Mouse drag simulates touch drag
- Mouse scroll simulates pinch zoom
- Left mouse button simulates touch tap

### Device Testing
For best results, test on actual Android devices:
```
File > Build Settings > Build and Run
```

## License
MIT