--Color = "red"
Color = {r = 255, g = 0, b = 0}
Name = "Stupid"

previousAngle = 0

enemyInd = 0

function Start()
	math.randomseed(123)
	enemys = BattleGround:GetEnemies()
	enemyInd = math.random(0, enemys.Length - 1)	
	curEnemy = enemys[enemyInd]
end

function Update()
	enemys = BattleGround:GetEnemies()
	if(enemys.Length <= 0) then
		return
	end
	
	if curEnemy.IsAlive == false then
		enemyInd = math.random(0, enemys.Length - 1)
		enemys = BattleGround:GetEnemies()
		curEnemy = enemys[enemyInd]
	end
	
	local x = curEnemy.X
	local y = curEnemy.Y
	
	local tX = Tank:GetX()
	local tY = Tank:GetY()
	
	local dist = math.sqrt((x - tX)^2 + (y - tY)^2)
	
	--local angle = math.atan2(y - tY, x - tX)
	--angle = math.deg(angle);
	local angle = GetDirectionCoords(tX, tY, x, y)
	
	if(Tank:GetHp() > 25.0 or dist > 10) then
		Tank:SetRotation(angle)
	else
		Tank:SetRotation(angle + 180)
	end
	
	angleD = angle - previousAngle
	Tank:SetTowerRotation(angle + angleD * dist / 6)
	previousAngle = angle

	if(dist > 5 or Tank:GetHp() < 25.0) then
		Tank:MoveForward()		
	end

	Tank:Shoot()
end
