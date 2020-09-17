# XWA-Multiplayer-Connector

## Background
A long time ago, in a country not too far away, some genius/madperson found a way to play the X-Wing Alliance solo campaign co-operatively in the multiplayer mode. Unfortunately their solution is a bit of a nuisance as it required each player alt tabbing during the game and copying a file quickly before the game disconnects. This application simplifies most of the hassle so it’s easier to enjoy.

## What the application does
It allows you to host a "co-ordinator" server, which isn't the actual game server, it's just a helper tool that the host can use to get the clients to automatically copy the right game file at the right time. It doesn't actually send the files, it just sends an instruction "copy mission x" and each players computer will copy the right local file to the right place.

## How to use
1. First time running, everyone will need to click the "Setup" button to identify where XWA is installed (probably located in C:\Program Files\Steam\steamapps\common\Star Wars X-Wing Alliance)
1. The person hosting the XWA game should run the application and choose "Host Server" and tell the players what their IP is (it shows what your external and internal IP is on the text box log)
1. All players must then run the application and join the host's server, they can then minimise the application and forget about it. Only the host needs to interact with it during the game.
1. Everyone then launches XWA and the host creates a lobby and gets all the player to join
1. The host then chooses the mission template and assigns players to the ship slots
1. Launch the mission
1. You should see 2 hyperspace buoys, everybody then presses "q" and "spacebar" to leave the mission, **BUT, stay in the Score Screen, don't go back further**.
1. From the score screen, the host must alt tab to the helper application, select the correct mission and click "Send out temp.tie". Wait to see all the players in the list go green however don't take too long or you'll get disconnected.
1. Go back to X-Wing Alliance
1. The host then clicks the relaunch button in the bottom-right corner. Don't go back to the main SKIRMISH screen, just click on the relaunch button from the score screen.
1. Enjoy the mission!

## Thanks
* Thanks to http://amiralaria.free.fr/xwam_an.php for the mission files and the guide
