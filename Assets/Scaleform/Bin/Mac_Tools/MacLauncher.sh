#!/bin/sh

# This script is used by the Scaleform Launcher to launch a terminal window
# alongside the FxMediaPlayer.  The output that you would typically see
# in Flash Studio will be displayed in the terminal window.

path_to_bin_dir=`dirname $0`
sh_file="$path_to_bin_dir/MacLauncher_1.sh"

# Generate temporary script
echo "#!/bin/sh" > $sh_file
echo "#" >> $sh_file
echo "# THIS IS AN AUTO GENERATED FILE BY MacLauncher.sh" >> $sh_file
echo "#" >> $sh_file
echo "$path_to_bin_dir/FxPlayer.app/Contents/MacOS/FxPlayer $*" >> $sh_file

# Run temporary script
chmod +x $sh_file
open -a Terminal.app $sh_file
