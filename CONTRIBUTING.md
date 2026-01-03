# Contributing to Dark Legend

Thank you for your interest in contributing to Dark Legend! This document provides guidelines for contributing to the project.

## Code of Conduct

- Be respectful and inclusive
- Provide constructive feedback
- Focus on the project goals
- Help others learn and grow

## How to Contribute

### Reporting Bugs

1. Check if the bug has already been reported in Issues
2. Create a new issue with:
   - Clear, descriptive title
   - Steps to reproduce
   - Expected vs actual behavior
   - Unity version and platform
   - Screenshots if applicable
   - Error messages or logs

### Suggesting Features

1. Check if the feature has been suggested
2. Create a feature request issue with:
   - Clear description of the feature
   - Use cases and benefits
   - Possible implementation approach
   - Any relevant examples

### Pull Requests

1. **Fork the Repository**
   ```bash
   git clone https://github.com/your-username/dark-legend.git
   cd dark-legend
   ```

2. **Create a Branch**
   ```bash
   git checkout -b feature/your-feature-name
   # or
   git checkout -b fix/bug-description
   ```

3. **Make Changes**
   - Follow existing code style
   - Add comments (English + Vietnamese)
   - Test your changes
   - Update documentation if needed

4. **Commit Changes**
   ```bash
   git add .
   git commit -m "Clear description of changes"
   ```

5. **Push to Fork**
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Create Pull Request**
   - Provide clear description
   - Reference related issues
   - Explain what was changed and why
   - Include screenshots if UI changes

## Code Style Guidelines

### C# Conventions

```csharp
// Use DarkLegend namespace
namespace DarkLegend.SystemName
{
    /// <summary>
    /// English description
    /// Mô tả tiếng Việt
    /// </summary>
    public class ClassName
    {
        // Public fields use PascalCase
        public int MaxHealth;
        
        // Private fields use camelCase with underscore
        private int _currentHealth;
        
        // Properties use PascalCase
        public int CurrentHealth { get; set; }
        
        /// <summary>
        /// Method description
        /// Mô tả phương thức
        /// </summary>
        public void MethodName()
        {
            // Implementation
        }
    }
}
```

### Unity Specific

- Use `[SerializeField]` for private fields exposed in Inspector
- Use `[Header("Section")]` to organize Inspector fields
- Use ScriptableObjects for data configuration
- Follow component-based architecture
- Use object pooling for frequently spawned objects

### Comments

- Add XML documentation comments for public APIs
- Include both English and Vietnamese descriptions
- Explain complex logic
- Document parameters and return values

```csharp
/// <summary>
/// Calculate damage with defense reduction
/// Tính sát thương với giảm phòng thủ
/// </summary>
/// <param name="baseDamage">Base damage value</param>
/// <param name="defense">Target's defense</param>
/// <returns>Final damage after reduction</returns>
public int CalculateDamage(int baseDamage, int defense)
{
    // Implementation
}
```

## Project Structure

When adding new features:

1. **Scripts** - Place in appropriate subfolder:
   - `Character/` - Player related
   - `Combat/` - Combat mechanics
   - `Enemy/` - Enemy AI and behavior
   - `Inventory/` - Items and equipment
   - `UI/` - User interface
   - `Managers/` - System managers
   - `Utils/` - Utilities and helpers

2. **ScriptableObjects** - Configuration data:
   - `ScriptableObjects/Classes/`
   - `ScriptableObjects/Skills/`
   - `ScriptableObjects/Items/`
   - `ScriptableObjects/Enemies/`

3. **Prefabs** - Reusable objects:
   - `Prefabs/Player/`
   - `Prefabs/Enemies/`
   - `Prefabs/Effects/`
   - `Prefabs/UI/`

## Testing

Before submitting PR:

1. Test in Unity Editor
2. Check for console errors
3. Test all affected features
4. Verify no breaking changes
5. Test on different resolutions (PC)

## Documentation

Update documentation when:

- Adding new features
- Changing existing behavior
- Adding new systems
- Modifying setup process

Files to update:
- README.md - Main documentation
- SETUP.md - Setup instructions
- CHANGELOG.md - Version changes
- Code comments

## Git Commit Messages

Format:
```
Type: Brief description

Detailed explanation if needed

Fixes #issue_number
```

Types:
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation
- `style:` - Code style changes
- `refactor:` - Code refactoring
- `test:` - Adding tests
- `chore:` - Maintenance

Examples:
```
feat: Add critical hit visual effect

Added particle effect when critical hits occur
Modified DamageCalculator to trigger effect event

Fixes #123
```

## What to Contribute

### High Priority
- Character 3D models
- Skill visual effects
- Enemy models and animations
- UI artwork and icons
- Sound effects and music
- Additional skills per class

### Medium Priority
- More enemy types
- New items and equipment
- Additional character classes
- New maps and zones
- Quest system
- NPC system

### Low Priority
- Gamepad support
- Additional languages
- Achievement system
- Advanced graphics options

## Questions?

- Open an issue for questions
- Check existing documentation
- Review code examples
- Ask in discussions

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

Thank you for contributing to Dark Legend! 🗡️
