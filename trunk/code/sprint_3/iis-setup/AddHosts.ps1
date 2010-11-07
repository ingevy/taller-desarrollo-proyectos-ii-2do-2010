# Adds a new host entry on the hosts file as shown:
#
# add-host 127.0.0.1 www.google.com [optional $true|$false]
#
# the third parameter indicates whether if exists it should be overwritten
function Add-Host {
 # map parameters
 $hostname, $address, $override = $args[0], $args[1], ($args[2] -eq $true)
 
 # generte some basics
 $hostsPath = $env:windir + "\System32\drivers\etc\hosts"
 $hosts, $entry = (gc $hostsPath), "$hostname $address"

 if([regex]::match($hosts, "\s*$hostname\s*").success){
  # it already existed but chose not override
  if(-not $override) { return; }
  clear-content $hostsPath
  $hosts | where-object{[regex]::match($_, "\s*$hostname\s.*$").success -eq $false} | add-content $hostsPath
 }
 
 # now append it
 add-content $hostsPath $entry
}

Add-Host "127.0.0.1" "callcenter.selfmanagement.com" $true
 