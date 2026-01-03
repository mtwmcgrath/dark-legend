# PhotonServerSettings Configuration Template

## File Location
This file should be created by Photon PUN 2 at:
`Assets/Photon/PhotonUnityNetworking/Resources/PhotonServerSettings.asset`

## Configuration Guide

### 1. App Settings

#### App ID PUN
```
[YOUR_APP_ID_HERE]
```
Get this from: https://dashboard.photonengine.com/

#### App Version
```
1.0
```
Used to separate different versions of your game. Players with different versions won't see each other.

#### Fixed Region
```
asia
```
Options: `asia`, `us`, `eu`, `jp`, `au`, `sa`, `kr`, etc.

For Vietnam and Southeast Asia, use: **`asia`**

#### Protocol
```
UDP
```
Options: UDP (recommended), TCP, WebSocket

UDP is fastest and recommended for games.

### 2. PUN Settings

#### Auto Join Lobby
```
✓ Enabled
```
Automatically joins lobby when connected to master server.

#### Enable Lobby Statistics
```
✓ Enabled
```
Receives room list updates in lobby.

#### Network Logging
```
Informational (or Full for debugging)
```
Options: Off, ErrorsOnly, Informational, Full

### 3. Dev Region

Set this to your preferred region during development:
```
asia
```

### 4. RPC List

Add these RPCs to the list (or let them auto-register):
- `RPC_Attack`
- `RPC_DealDamage`
- `RPC_CastSkill`
- `RPC_CastSkillOnTarget`
- `RPC_ApplyBuff`
- `RPC_ApplyDebuff`
- `RPC_Die`
- `RPC_Respawn`
- `RPC_Teleport`
- `RPC_RoomMessage`
- `RPC_PlayAnimation`
- `RPC_DropLoot`
- `RPC_ChangeState`
- `RPC_UpdateName`
- `RPC_UpdateHealth`

### 5. Prefab List

Add these prefabs to instantiate them via PhotonNetwork.Instantiate():
- `NetworkPlayer`
- `NetworkPlayer_DarkKnight` (optional)
- `NetworkPlayer_DarkWizard` (optional)
- `NetworkPlayer_Elf` (optional)
- `NetworkEnemy`
- `NetworkEnemy_Goblin` (optional)
- `NetworkEnemy_Orc` (optional)
- `NetworkEnemy_Dragon` (optional)

> **Important**: All prefabs must be in the `Assets/Resources/` folder or a subfolder of it.

### 6. Start In Offline Mode
```
✗ Disabled
```
Only enable for offline testing.

### 7. Reliable UDP Settings

#### Max Message Size
```
1200 bytes (default)
```

#### Sent Count Allowance
```
7 (default)
```

### 8. Fallback Protocol

#### Use Alternative UDP Ports
```
✓ Enabled (if you have firewall issues)
```

#### Alternative Ports
```
27000-27030 (default range)
```

## Example Configuration (JSON representation)

```json
{
  "AppSettings": {
    "AppIdRealtime": "YOUR_APP_ID_HERE",
    "AppIdChat": "",
    "AppIdVoice": "",
    "AppVersion": "1.0",
    "UseNameServer": true,
    "FixedRegion": "asia",
    "Server": "",
    "Port": 0,
    "ProxyServer": "",
    "Protocol": 0,
    "EnableProtocolFallback": true,
    "AuthMode": 0,
    "EnableLobbyStatistics": true,
    "NetworkLogging": 1
  },
  "DevRegion": "asia",
  "PunLogging": 1,
  "EnableSupportLogger": false,
  "RunInBackground": true,
  "StartInOfflineMode": false,
  "RpcList": [
    "RPC_Attack",
    "RPC_DealDamage",
    "RPC_CastSkill",
    "RPC_Teleport",
    "RPC_Die",
    "RPC_Respawn"
  ]
}
```

## Region Codes Reference

| Region Code | Location | Best For |
|------------|----------|----------|
| `asia` | Singapore | Southeast Asia, Vietnam |
| `jp` | Japan | Japan, East Asia |
| `kr` | South Korea | Korea |
| `us` | USA East | Americas |
| `usw` | USA West | West Coast USA |
| `eu` | Europe | Europe, Middle East |
| `au` | Australia | Australia, New Zealand |
| `sa` | Brazil | South America |
| `in` | India | India |

## Setting Up in Unity Editor

### Method 1: PUN Wizard (Recommended)
1. Go to **Window > Photon Unity Networking > PUN Wizard**
2. Click **Setup Project**
3. Enter your App ID
4. Click **Setup**
5. The wizard will create and configure PhotonServerSettings

### Method 2: Manual Setup
1. Import Photon PUN 2 from Asset Store
2. Locate `PhotonServerSettings.asset` in Project window
3. Click on it to view in Inspector
4. Fill in the settings as described above

### Method 3: Via Script
```csharp
using Photon.Pun;
using Photon.Realtime;

public class PhotonSetup : MonoBehaviour
{
    void SetupPhoton()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "YOUR_APP_ID";
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
        PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = "1.0";
        PhotonNetwork.PhotonServerSettings.AppSettings.Protocol = ConnectionProtocol.Udp;
        PhotonNetwork.GameVersion = "1.0";
    }
}
```

## Verification

After setup, verify your configuration:

```csharp
void Start()
{
    Debug.Log($"App ID: {PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime}");
    Debug.Log($"Region: {PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion}");
    Debug.Log($"Version: {PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion}");
}
```

## Common Issues

### "Invalid App ID"
- Check that App ID is correctly copied from Photon Dashboard
- Ensure no extra spaces or characters
- Verify App Type is "PUN" not "Fusion" or "Chat"

### "Cannot connect to region"
- Check internet connection
- Try different region
- Check Photon server status: https://status.photonengine.com/

### "Prefab not found"
- Ensure prefab is in `Assets/Resources/` folder
- Check prefab name matches exactly (case-sensitive)
- Add prefab to PhotonServerSettings prefab list

## Best Practices

1. **Use Environment Variables for App ID** (for version control)
   ```csharp
   string appId = Environment.GetEnvironmentVariable("PHOTON_APP_ID");
   PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = appId;
   ```

2. **Different App IDs for Dev/Prod**
   - Development: One App ID
   - Production: Another App ID
   - Prevents mixing test and production players

3. **Version Control**
   - Add `PhotonServerSettings.asset` to `.gitignore` if it contains sensitive data
   - Or use placeholder App ID in version control

4. **Region Selection**
   - Let players choose region in game
   - Auto-detect best region using ping
   - Fall back to closest region if preferred is unavailable

## Support

- Photon Documentation: https://doc.photonengine.com/
- Photon Dashboard: https://dashboard.photonengine.com/
- Photon Forum: https://forum.photonengine.com/
- Photon Discord: https://discord.gg/photonengine

---

**Remember**: Keep your App ID secure and don't share it publicly!
