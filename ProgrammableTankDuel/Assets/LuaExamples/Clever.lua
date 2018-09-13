Color = {r = 0, g = 0, b = 255}
Name = "Clever"

State = 0 -- 0 - moving, 1 - changing trajectory, 2 - dodging, 3 - borders
msgNum = 0

radius = 15

x = math.random(0, 15)
y = 0
angle = 0
prevHealth = 100

backward = false

actionTime = 0

sayShellImapact = {'AAA, Ya maslinu poymal!', 'Sooka ti cho samiy metkiy??', 'Noo vse peezda tebe', 'Blyad!', 'Suka', 'Pizdec', 'Govna-piroga'}
sayRandom = {'Penis', 'Blya))', 'Normas gonyaem, pazani', 'Porvoo kak loha'}
sayGreeting = {'Zdarova, parni', 'Seychas dam pososat :)', 'Ya Misha', 'Privet c:', 'Dratuti', 'Darov', 'Priv', 'Cho kovo'}

function OnHealthLoss()
	Messager:PrintRawTimed(GetRandomPhrase(sayShellImapact), 2)
end

function GetRandomPhrase(arr)
	local index = math.random(1, #arr - 1);	
	local phrase = arr[index]
	--local phrase = string.format('[TEST PHRASE]: %d out of %d', index, #arr)
	return phrase
end

function Start()
	prevHealth = Tank:GetHp()
	local phrase = GetRandomPhrase(sayGreeting)	
	Messager:PrintRawTimed(phrase, 3)
end

prevAngle = 0
function Aim()
	local tarAngle = GetDirectionCoords(posx, posy, enemy.X, enemy.Y)	
	local distance = math.sqrt((enemy.X - posx)^2 + (enemy.Y - posy)^2)

	local angleD = tarAngle - prevAngle
	
	local angleShift = angleD * distance / 6
	
	local halfWidth = (enemy.Width / 2)
	
	local bodyAngleSize = math.deg(math.atan(halfWidth / distance))
	

	if(math.abs(angleShift) <= bodyAngleSize) then
		angleShift = 0
	end		

	Tank:SetTowerRotation(tarAngle + angleShift)
	
	if(FindAlliesOnShoot(Tank:GetTowerRotation()) == false) then
		Tank:Shoot()
	end
	prevAngle = tarAngle
end

function FindDangerousShells()
	local shells = BattleGround:GetShells()
	local ln = shells.Length
	for i = 0, ln - 1 do
		local shell = shells[i]	
		
		local toShellAngle = GetDirectionCoords(posx, posy, shell.X, shell.Y)
		local diff = math.abs(AngleDifference(toShellAngle, shell.Direction + 180))	
				
		if(math.abs(diff) < 20) then
			return true;
		end
	end
	return false;
end

function FindAllyCollisions()
	local ln = allies.Length
	for i = 0, ln - 1 do
		local dist = math.sqrt((allies[i].X - posx)^2 + (allies[i].Y - posy)^2)
		if dist < 5.0 then
			return true, allies[i]
		end
	end
	
	local ln = enemies.Length
	for i = 0, ln - 1 do
		local dist = math.sqrt((enemies[i].X - posx)^2 + (enemies[i].Y - posy)^2)
		if dist < 5.0 then
			return true, enemies[i]
		end
	end
	
	return false, nil
end

function FindAlliesOnShoot(curAngle)
	local ln = allies.Length
	for i = 0, ln - 1 do
		local toAllyAngle = GetDirectionCoords(posx, posy, allies[i].X, allies[i].Y)
		local towerRot = curAngle
		if(math.abs(AngleDifference(toAllyAngle, towerRot)) < 15) then
			return true
		end
	end
	return false
end

function GetRange(x1, y1, x2, y2)
	return math.sqrt((x2 - x1)^2 + (y2 - y1)^2)
end

function FindNearest()
	local ln = enemies.Length
	local mRange = GetRange(Tank:GetX(), Tank:GetY(), enemies[0].X, enemies[0].Y)
	local mIndex = 0
	for i = 0, ln - 1 do
		local range = GetRange(Tank:GetX(), Tank:GetY(), enemies[i].X, enemies[i].Y)
		if range < mRange then
			mRange = range
			mIndex = i
		end
	end	
	return mIndex, mRange
end

function FindWeakerest()
	local ln = enemies.Length
	local mHp = enemies[0].HP
	local mIndex = 0
	for i = 0, ln - 1 do
		local hp = enemies[0].HP
		if hp < mHp then
			mHp = hp
			mIndex = i
		end
	end	
	return mIndex
end

function Update()

	if(prevHealth > Tank:GetHp()) then
		OnHealthLoss()		
	end
	
	prevHealth = Tank:GetHp()
	
	posx = Tank:GetX()
	posy = Tank:GetY()
	allies = BattleGround:GetAllies()
	enemies = BattleGround:GetEnemies()
	if(enemies.Length <= 0) then
		return
	end
	--if(enemy == nil or enemy.IsAlive == false or GetRange(enemy.X, enemy.Y, posx, posy) > 8) then
		local index, range = FindNearest()
		local mHpI = FindWeakerest()
		local mHpRange = GetRange(posx, posy, enemies[mHpI].X, enemies[mHpI].Y)
		if(mHpRange < 10) then		
			enemy = enemies[mHpI]
		else
			enemy = enemies[index]
		end
	--end
	Aim()
	
	-- Checking game area
	if(posx > 30 or posy > 25) then
		actionTime = 8
		State = 3
	end
	
	-- Searching for collisions
	collides, collider = FindAllyCollisions()
	if(collides) then
		--Out:PrintRaw("ALLY!")
		--Out:Endl()
		if(actionTime <= 0) then
			actionTime = 6
			State = 2
			if(collider ~= nil) then
				local angle = GetDirectionCoords(posx, posy, collider.X, collider.Y)			
				--Tank:SetRotation(angle + 180)
				local diff = math.abs(AngleDifference(angle, Tank:GetRotation()))
				if(diff > 150) then
					backward = false
					Tank:SetRotation(angle + 180)
				else
					backward = true
					Tank:SetRotation(angle)
				end
			end			
		end
	end
	
	danger, shell = FindDangerousShells()
	if(danger) then
		if(actionTime <= 0) then
			traj_shiftSet = false
			State = 1
			actionTime = 3
		end	
	end
	
	if(State == 2) then
		if(backward) then
			Tank:MoveBackward()			
		else
			Tank:MoveForward()	
		end
	end	
	
	if(State == 0) then
		--if(math.sqrt((x - posx)^2 + (y - posy)^2) < 5) then
			local angleStet = 90
			local radius = math.random(6, 9)
			local x = enemy.X + radius * math.cos(math.rad(angle))
			local y = enemy.Y + radius * math.sin(math.rad(angle))
		--end			
		Tank:MoveTo(x, y)
	end
	
	if State == 1 then -- Changing trajectory
		if(traj_shiftSet == false) then
			local shStat = math.random(-1, 1)
			--local shellAngle = GetDirectionCoords(shell.X, shell.Y, posx, posy)
			--local shellDir = shell.Direction
			
			if(shStat > 0) then
				traj_angShift = Tank:GetRotation() + 90
			else
				traj_angShift = Tank:GetRotation() - 90
			end
			--traj_dir = math.random(0, 1)			
			traj_shiftSet = true	
		else
			Tank:SetRotation(traj_angShift)
		end
		
		Tank:MoveForward()
	end
	
	if State == 3 then
		Tank:MoveTo(0, 0)
	end
		
	if actionTime > 0 then
		actionTime = actionTime - 1
	else
		actionTime = 0
		State = 0
	end
	
end