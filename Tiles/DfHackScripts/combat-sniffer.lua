
function RegisterForEvents()
	eventful = require "plugins.eventful"
	eventful.enableEvent(eventful.eventType.REPORT, 0)
	eventful.enableEvent(eventful.eventType.UNIT_ATTACK, 0)
	eventful.onReport.something=function(reportId)
		OnReport(reportId)
	end
	eventful.onUnitAttack.something = function(attackerId, defenderId, woundId)
		Append("UNIT_ATTACK_START")
		OnUnitAttack(attackerId, defenderId, woundId)
		Append("UNIT_ATTACK_END")
		Append("")
	end
end


function UnitName(unit)
	return unit.name.first_name
end

function UnitSize(unit)
	return unit.body.size_info.size_cur
end

function UnitStrength(unit)
	return unit.body.physical_attrs.STRENGTH.value
end

function OnReport(reportId)
	local report=df.report.find(reportId)

	local text = report.text
	if(NeedsContinue(report)) then
		AppendLastReportText(text)
	else
		ShowLastReportText()
		AddReportText(text)
	end

end


function AddReportText(text)
	table.insert(ReportTexts, text)
end

function AppendLastReportText(text)
	i = #ReportTexts
	ReportTexts[i] = ReportTexts[i] .. " " .. text
end

function ShowLastReportText()
	i = #ReportTexts
	if(i > 0) then
		Append("REPORT_TEXT: " .. ReportTexts[i])
	end
	ReportTexts = {}
end

function NeedsContinue(report)
	return report.flags.continuation or report.flags.unconscious
end

function IsAttackText(text)
	return true
end

function OnUnitAttack(attackerId, defenderId, woundId)
	local attacker = GetUnit(attackerId)
	local defender = GetUnit(defenderId)
	
	Append("Attacker ID: " .. attackerId)
	Append("ATTACKER: " .. UnitName(attacker))
	Append("ATTACKER_SIZE:" .. UnitSize(attacker))
	Append("ATTACKER_STRENGTH: " .. UnitStrength(attacker))
	
	--DumpAttacks(attacker)

	Append("Defender ID: " .. defenderId)
	Append("DEFENDER: " .. UnitName(defender))
	Append("DEFENDER_SIZE:" .. UnitSize(defender))
	Append("DEFENDER_STRENGTH: " .. UnitStrength(defender))

	Append("Wound ID: " .. woundId)
	if(woundId ~= -1) then
		local wound = GetWound(defender, woundId)

		Append("DEFENDER_WOUND_START")
		if(wound ~= nil) then
			DumpDefenderWound(defender, wound)
		else
			Append("Nil Wound")
		end
		Append("DEFENDER_WOUND_END")
	end

	CheckUnitDump(attackerId)
	CheckUnitDump(defenderId)
end


DumpedUnits = {}
function CheckUnitDump(unitId)
	if(DumpedUnits[unitId] == nil) then
		DumpUnit(unitId)
		DumpedUnits[unitId] = 1
	end
end


function DumpUnit(unitId)
	local unit = GetUnit(unitId)

	Append("UNIT_START")
	Append("ID: " .. unitId)
	Append("NAME:" .. UnitName(unit))
	Append("SIZE:" .. UnitSize(unit))
	Append("STRENGTH: " .. UnitStrength(unit))

	-- TODO - dump skills/attributes
	DumpBody(unit)
	DumpAttacks(unit)

	Append("UNIT_END")
end

function DumpBody(unit)
	local layer_part =unit.body.body_plan.layer_part
	local body_plan = unit.body.body_plan
	local size_info = unit.body.size_info
	local phys = unit.body.physical_attrs

	Append("BODY_START")
	Append("SIZE_CURRENT: " .. size_info.size_cur)
	Append("SIZE_BASE: " .. size_info.size_base)
	Append("AREA_CURRENT: " .. size_info.area_cur)
	Append("AREA_BASE: " .. size_info.area_base)
	Append("LENGTH_CURRENT: " .. size_info.length_cur)
	Append("LENGTH_BASE: " .. size_info.length_base)
	Append("TOTAL_RELSIZE: " .. body_plan.total_relsize)
	Append("PHYSICAL_STRENGTH: " .. phys.STRENGTH.value)
	Append("PHYSICAL_AGILITY: " .. phys.AGILITY.value)
	Append("PHYSICAL_TOUGHNESS: " .. phys.TOUGHNESS.value)
	Append("PHYSICAL_ENDURANCE: " .. phys.ENDURANCE.value)
	Append("PHYSICAL_RECUPERATION: " .. phys.RECUPERATION.value)
	Append("PHYSICAL_DISEASE_RESISTANCE: " .. phys.DISEASE_RESISTANCE.value)
	Append("BLOOD_MAX: " .. unit.body.blood_max)
	Append("BLOOD_COUNT: " .. unit.body.blood_count)

	for body_part_id,body_part in pairs(body_plan.body_parts) do
		Append("BODY_PART_START")

		Append("NAME_SINGULAR:" .. body_part.name_singular[0].value)
		Append("NAME_PLURAL:" .. body_part.name_plural[0].value)
		Append("TOKEN: " .. body_part.token)
		Append("RELSIZE: " .. body_part.relsize)
		Append("FRACTION_TOTAL:" .. body_part.fraction_total)
		Append("FRACTION_BASE:" .. body_part.fraction_base)
		Append("FRACTION_FAT:" .. body_part.fraction_fat)
		Append("FRACTION_MUSCLE:" .. body_part.fraction_muscle)

		for layerIdx, layer in pairs(body_part.layers) do
			DumpTissue(unit, body_part_id, layer)
		end

		Append("BODY_PART_END")
	end

	Append("BODY_END")
end

function GetLastUnitReport(unit)
	local reports = unit.reports.log.Combat;

	local reportIdx = TableLength(reports) - 1

	local reportId =  reports[reportIdx]
	local report=df.report.find(reportId)
	local text = report.text
	while(report.flags.continuation or report.flags.unconscious) do
		reportIdx = reportIdx - 1
		reportId = reports[reportIdx]
		report=df.report.find(reportId)
		text = report.text .. " " .. text
	end

	return text
end

function FlagString(flag)
	return Ternary(flag, "true", "false")
end

function DumpDefenderWound(unit, wound)
	age = wound.age -- might require = 0 for "new" wound

	Append("WOUND_AGE: " .. age)
	parts = wound.parts
	numParts = #parts
	Append("WOUND_PARTS: " .. numParts)

--        <bitfield name="flags">
--            <flag-bit name='severed_part'/>
--            <flag-bit name='mortal_wound'/>
--            <flag-bit name='stuck_weapon'/>
--            <flag-bit name='diagnosed'/>
--            <flag-bit name='sutured'/>
--            <flag-bit name='infection'/>
--        </bitfield>

	Append("SEVERED_PART: " .. FlagString(wound.flags.severed_part))
	Append("MORTAL_WOUND: " .. FlagString(wound.flags.mortal_wound))
	Append("STUCK_WEAPON: " .. FlagString(wound.flags.stuck_weapon))
	Append("DIAGNOSED: " .. FlagString(wound.flags.diagnosed))
	Append("SUTURED: " .. FlagString(wound.flags.sutured))
	Append("INFECTION: " .. FlagString(wound.flags.infection))

--        <int32_t name="attacker_unit_id" ref-target='unit'/>
--        <int32_t name="attacker_hist_figure_id" ref-target='historical_figure'/>
--<int32_t name="syndrome_id" ref-target='syndrome'/>

--        <int32_t name="pain"/>
--        <int32_t name="nausea"/>
--        <int32_t name="dizziness"/>
--      <int32_t name="paralysis"/>
--        <int32_t name="numbness"/>
--        <int32_t name="fever"/>
--        <pointer name="curse" type-name='wound_curse_info'/>

	Append("ATTACKER_UNIT_ID: " .. wound.attacker_unit_id)
	Append("ATTACKER_HIST_FIGURE_ID: " .. wound.attacker_hist_figure_id)
	Append("SYNDROME_ID: " .. wound.syndrome_id)
	Append("PAIN: " .. wound.pain)
	Append("FEVER: " .. wound.fever)



	for i=1, numParts, 1 do
		Append("WOUND_BODY_PART_START")
		part = parts[i-1]
		bpId = part.body_part_id
		layer_idx = part.layer_idx
		Append("BODY_PART_ID: " .. bpId)
		Append("LAYER_IDX:" .. layer_idx)
		Append("GLOBAL_LAYER_IDX: " .. part.global_layer_idx)
		Append("CONTACT_AREA: " .. part.contact_area)
		Append("SURFACE_PERC: " .. part.surface_perc)
		Append("STRAIN: " .. part.strain)
		Append("CUR_PENETRATION_PERC: " .. part.cur_penetration_perc)
		Append("MAX_PENETRATION_PERC: " .. part.max_penetration_perc)
		Append("JAMMED_LAYER_IDX: " .. part.jammed_layer_idx)
--		Append("EDGED_CURV_PERC: " .. part.edged_curv_perc)
		Append("BLEEDING: " .. part.bleeding)
		Append("PAIN: " .. part.pain)
		Append("NAUSEA: " .. part.nausea)
		Append("DIZZINESS: " .. part.dizziness)
		Append("PARALYSIS: " .. part.paralysis)
		Append("NUMBNESS: " .. part.numbness)
		Append("SWELLING: " .. part.swelling)
		Append("IMPAIRED: " .. part.impaired)

		wp = part
		bpI = wp.body_part_id
		layerI = wp.layer_idx
		gLayerI = wp.global_layer_idx

		bp = unit.body.body_plan.body_parts[bpI]
		bpName = bp.name_singular[0].value
		
		Append("BODY_PART_NAME_SINGULAR: " .. bpName)
		Append("BODY_PART_NAME_PLURAL: " .. bp.name_plural[0].value)
		if(layerI ~= -1) then
			layer = bp.layers[layerI]
			layerName = layer.layer_name
			cut = unit.body.components.layer_cut_fraction[gLayerI]
			dent = unit.body.components.layer_dent_fraction[gLayerI]
			effect = unit.body.components.layer_effect_fraction[gLayerI]
	
			woundArea = unit.body.components.layer_wound_area[gLayerI]

			Append("LAYER_NAME: " .. layerName)
			Append("CUT_FRACTION:" .. cut)
			Append("DENT_FRACTION: " .. dent)
			Append("EFFECT_FRACTION:" .. effect)
			Append("LAYER_WOUND_AREA:" .. woundArea)
--			DumpTissue(unit, bpI, layerName)
		else
--			Append("NO_TISSUE_LAYER_DEFINED")
		end
		
		Append("WOUND_BODY_PART_END")
	end
end

function DumpTissues(unit, body_part_id)
	local layer_part =unit.body.body_plan.layer_part

	for k,v in pairs(layer_part) do
		if(v == body_part_id) then
			DumpTissue(unit, body_part_id, k)
		end
	end
end

function ReflectDump(o)
	for rkey,rvalue in pairs(o) do
	    print("found member " .. rkey);
	end
end


function DumpTissue(unit, body_part_id, layer)
	local tissue_id = layer.tissue_id
	local v = unit.body.body_plan.body_parts[body_part_id]
	local cur_size = unit.body.size_info.size_cur
	local curstrength=unit.body.physical_attrs.STRENGTH.value
	local race = df.global.world.raws.creatures.all[unit.race]
	local partrelsize = v.relsize
	local bodyTotalRelSize = unit.body.body_plan.total_relsize
	local partsize = math.floor(cur_size * v.relsize / unit.body.body_plan.total_relsize)
	local partthick = math.floor((partsize * 10000) ^ 0.333)
	local contact = math.floor(partsize ^ 0.666)
	local bpName = v.name_singular[0].value
	local layer_part =unit.body.body_plan.layer_part
--	for kk,vv in pairs(v.layers) do
		local vv = layer
		tisdata=race.tissue[vv.tissue_id]
		layername = vv.layer_name

		--if(tisdata.id == tissue_id) then
			material=dfhack.matinfo.decode(tisdata.mat_type,tisdata.mat_index)
			--tissue has a mat_state, could it name properly
			matdata=material.material.strength
			
			modpartfraction= vv.part_fraction
			if tisdata.flags.THICKENS_ON_ENERGY_STORAGE == true then
				modpartfraction = unit.counters2.stored_fat * modpartfraction / 2500 / 100
			end
			if tisdata.flags.THICKENS_ON_STRENGTH == true then
				modpartfraction = curstrength * modpartfraction / 1000
			end
			layervolume = math.floor(partsize * modpartfraction / v.fraction_total)
			layerthick = math.floor(partthick * modpartfraction / v.fraction_total)
			if layervolume == 0 then
				layervolume = 1
			end
			if layerthick == 0 then
				layerthick = 1
			end
	
			vbca=layervolume*matdata.yield.IMPACT/100/500/10
			vbcb=layervolume*(matdata.fracture.IMPACT-matdata.yield.IMPACT)/100/500/10
			vbcc=layervolume*(matdata.fracture.IMPACT-matdata.yield.IMPACT)/100/500/10
			deduct= math.floor(vbca/10)
			if matdata.strain_at_yield.IMPACT >= 50000 then
				vbcb=0
				vbcc=0
			end
			fullbmr= math.floor(vbca+vbcb+vbcc)
	

			Append("TISSUE_LAYER_START")

			Append("TISSUE_ID:" .. tisdata.id)
			Append("LAYER: " .. vv.layer_name)
			Append("VOLUME: " .. layervolume)
			Append("CONTACT: " .. contact)
			Append("THICKNESS: " .. layerthick)
			Append("MATERIAL: " .. material.material.state_name.Solid)
			Append("BLUNT_MOMENTUM_RESIST: " .. fullbmr)
			Append("SHEAR_YIELD: " .. matdata.yield.SHEAR)
			Append("SHEAR_FRACTURE:" .. matdata.fracture.SHEAR)
			Append("PART_SIZE: " .. partsize) 
			Append("PART_THICKNESS: " .. partthick)
			Append("RELATIVE_PART_SIZE: " .. partrelsize)
			Append("TOTAL_BODY_RELATIVE_SIZE: " .. bodyTotalRelSize)

			Append("TISSUE_LAYER_END")
--		end
--	end
			
end

function DumpAttacks(unit)

	local cur_size = unit.body.size_info.size_cur
	local curstrength=unit.body.physical_attrs.STRENGTH.value
	local race = df.global.world.raws.creatures.all[unit.race]

	for k,v in pairs(unit.inventory) do
		vitype=df.item_type[v.item:getType()]

		material=dfhack.matinfo.decode(v.item)
		matdata=material.material.strength
		vmatname=material.material.state_name.Solid

		vbpart=unit.body.body_plan.body_parts[v.body_part_id]

		if vitype=="WEAPON" then
			Append("START_WEAPON")
			Append("MATERIAL_NAME: " .. vmatname)
			Append("ITEM_SUB_TYPE_NAME: ".. v.item.subtype.name)
			v.item:calculateWeight()
			effweight=unit.body.size_info.size_cur/100+v.item.weight*100+v.item.weight_fraction/10000
			actweight=v.item.weight*1000+v.item.weight_fraction/1000
			if v.item.subtype.flags.HAS_EDGE_ATTACK==true then
				Append("SHEAR_YIELD: ".. matdata.yield.SHEAR)
				Append("SHEAR_FRACTURE: " .. matdata.fracture.SHEAR)
				Append("SHARPNESS: " .. v.item.sharpness)
			end
	--	print("NAME", "EDGE", "CONTACT", "PNTRT", "WEIGHT", "VEL", "MOMENTUM(+500%/-50%)")
			for kk,vv in pairs(v.item.subtype.attacks) do

				vvel=unit.body.size_info.size_base * curstrength * vv.velocity_mult/1000/effweight/1000
				vmom=vvel*actweight/1000+1
				vedge="blunt"
				vcut=""
				if vv.edged==true then
					vedge="edged" 
					vcut=100
				end

				Append("START_WEAPON_ATTACK")
				Append("NAME: " .. vv.verb_2nd) 
				Append("EDGE: " .. vedge)
				Append("CONTACT: " .. vv.contact)
				Append("PENETRATION: " .. vv.penetration)
				Append("WEIGHT: " .. actweight/1000)
				Append("VELOCITY: " .. math.floor(vvel))
				Append("MOMENTUM: " .. math.floor(vmom))
				Append("END_WEAPON_ATTACK")
			
			end
			actvol=v.item:getVolume()
			Append("BLUNT_DEFLECT_IF_LAYER_WEIGHT_MORE_THAN: " .. actvol * matdata.yield.IMPACT / 100 / 500 / 1000)

			Append("END_WEAPON")
		else
			if v.mode==1 then
				Append("START_WEAPON")
				Append("MATERIAL_NAME: " .. vmatname)
				Append("ITEM_SUB_TYPE_NAME: (misc weapon)")
				--item held in hands treated as misc weapon
				--1000 velocity mod, power math for contact and penetration
				actvol=v.item:getVolume()
				v.item:calculateWeight()
				actweight=v.item.weight*1000+v.item.weight_fraction/1000
				effweight=unit.body.size_info.size_cur/100+v.item.weight*100+v.item.weight_fraction/10000
				misccontact=math.floor(actvol ^ 0.666)
				miscpene=math.floor((actvol*10000) ^ 0.333)
		--		print("NAME", "EDGE", "CONTACT", "PNTRT", "WEIGHT", "VEL", "MOMENTUM(+500%/-50%)")
				vvel=unit.body.size_info.size_base * curstrength/effweight/1000
				vmom=vvel*actweight/1000+1
				vedge="blunt"
	
				Append("START_WEAPON_ATTACK")
				Append("NAME: strike ")
				Append("EDGE: " .. vedge)
				Append("CONTACT: " .. misccontact)
				Append("PENETRATION: " .. miscpene)
				Append("WEIGHT: " .. actweight/1000)
				Append("VELOCITY: " .. math.floor(vvel))
				Append("MOMENTUM: " .. math.floor(vmom))
				Append("END_WEAPON_ATTACK")
	
				Append("BLUNT_DEFLECT_IF_LAYER_WEIGHT_MORE_THAN: ", actvol * matdata.yield.IMPACT / 100 / 500 / 1000)
	
				Append("END_WEAPON")
			end
		end

		if vitype=="ARMOR" or vitype=="HELM" or vitype=="GLOVES" or vitype=="SHOES" or vitype=="PANTS" then
			Append("START_ARMOR")
			Append("MATERIAL_NAME: " .. vmatname)
			Append("ITEM_SUB_TYPE_NAME: ".. v.item.subtype.name)
		--	print(vmatname, v.item.subtype.name)
			actvol=v.item:getVolume()
			v.item:calculateWeight()
			actweight=v.item.weight*1000+v.item.weight_fraction/1000
			vbca=actvol*matdata.yield.IMPACT/100/500/10
			vbcb=actvol*(matdata.fracture.IMPACT-matdata.yield.IMPACT)/100/500/10
			vbcc=actvol*(matdata.fracture.IMPACT-matdata.yield.IMPACT)/100/500/10
			deduct=vbca/10
			if matdata.strain_at_yield.IMPACT >= 50000 or v.item.subtype.props.flags.STRUCTURAL_ELASTICITY_WOVEN_THREAD==true 
				or v.item.subtype.props.flags.STRUCTURAL_ELASTICITY_CHAIN_METAL==true 
				or v.item.subtype.props.flags.STRUCTURAL_ELASTICITY_CHAIN_ALL==true then
				vbcb=0
				vbcc=0
			end
			Append("FULL_CONTACT_BLUNT_MOMENTUM_RESIST: " .. math.floor(vbca+vbcb+vbcc))
			Append("CONTACT_10_BLUNT_MOMENTUM_RESIST: " .. math.floor((vbca+vbcb+vbcc)*10/actvol))
			Append("UNBROKEN_MOMENTUM_DEFLECTION: " .. math.floor(deduct), math.floor(deduct*10/actvol))
			Append("VOLUME: " .. actvol)	
			Append("WEIGHT:  " .. actweight/1000)
			vshyre=matdata.yield.SHEAR
			vshfre=matdata.fracture.SHEAR
			if v.item.subtype.props.flags.STRUCTURAL_ELASTICITY_WOVEN_THREAD==true and vmatname ~= "leather" then
				if vshyre>20000 then vshyre=20000 end
				if vshfre>30000 then vshfre=30000 end
			end
			Append("SHEAR_YIELD: " ..  vshyre)
			Append("SHEAR_FRACTURE: " .. vshfre)
		
			Append("END_ARMOR")
		end
	end    --end of unit inventory loop


	--print("BODY PART ATTACKS")
	--print("NAME", "EDGE", "SIZE", "CONTACT", "PNTRT", "WEIGHT", "VEL", "MOMENTUM(+500%/-50%)", "SOLID DENSITY")

	for k,v in pairs(unit.body.body_plan.attacks) do
		Append("START_BODY_PART_ATTACK")
		attackpart=unit.body.body_plan.body_parts[v.body_part_idx[0]]
		if v.tissue_layer_idx[0] == -1 then
			--normal no chosen tissue
			layerdata=attackpart.layers[#attackpart.layers-1] --is it always the last layer???
		else
			--scratch nails etc.
			layerdata=attackpart.layers[v.tissue_layer_idx[0]]
		end
		tisdata=race.tissue[layerdata.tissue_id]
		material=dfhack.matinfo.decode(tisdata.mat_type,tisdata.mat_index)
		matdata=material.material.strength
	
		sumrelsize=0
		for kk,vv in pairs(v.body_part_idx) do
			sumrelsize=sumrelsize + unit.body.body_plan.body_parts[vv].relsize
		end

		partsize = math.floor(cur_size * sumrelsize / unit.body.body_plan.total_relsize)
		contact = math.floor((partsize ^ 0.666) * v.contact_perc/100)
		attpene = math.floor(partsize * v.penetration_perc/100)
		partweight = math.floor(partsize * material.material.solid_density / 100)
		vvel = 100 * curstrength / 1000 * v.velocity_modifier / 1000
		vmom = vvel * partweight / 1000 + 1

		vedge="blunt"
		if v.flags.edge==true then
			vedge="edged" 
		end
		Append("NAME: " .. v.name)
		Append("EDGE: " .. vedge)
		Append("SIZE: " .. partsize)
		Append("CONTACT: " .. contact)
		Append("PENETRATION: " .. attpene) 
		Append("WEIGHT: " .. partweight/1000)
		Append("VELOCITY: " .. math.floor(vvel))
		Append("MOMENTUM: " .. math.floor(vmom)) 
		Append("MATERIAL_NAME: " .. material.material.state_name.Solid)
		Append("SOLID_DENSITY: " .. material.material.solid_density)

		if v.flags.edge==true then
			Append("SHEAR_YIELD: " ..  matdata.yield.SHEAR)
			Append("SHEAR_FRACTURE: " .. matdata.fracture.SHEAR)
		end
		--print(" ")
		--
		Append("END_BODY_PART_ATTACK")
	end
end




function GetWound(unit, woundId)
	local l = TableLength(unit.body.wounds)

	for i=1, l do
		local id = unit.body.wounds[i-1].id
		if(id == woundId) then
			return unit.body.wounds[i-1]
		end
	end
	return nil
end


function GetUnit(id)
	local unit = df.unit.find(id)
	if not unit then
	  error('invalid unit!', args.unit)
	end
	return unit
end

function Append(str)
	fileName = "combat-sniffer-log.txt"
	file = io.open(fileName, "a")
	io.output(file)
	io.write(str .. "\n")
	io.close(file)
end

function IsStrikeReport(text)
	return true
end

function SplitWords(text)
	words = {}
	for word in string.gmatch(text, "%w+") do table.insert(words, word) end
	return words
end

function Ternary ( cond , T , F )
    if cond then return T else return F end
end

function TableLength(T)
--	return #T
  local count = 0
  for _ in ipairs(T) do count = count + 1 end
  return count
end

function GetCapitalizedGroups(text)
	groups = {}

	curGroupIndex = 0
	curInCap = 0

	words = SplitWords(text)
	for i=1, #words do
		wordIsCap = Ternary(string.find(words[i], "^%u"), 1, 0)

		if(curInCap == wordIsCap) then
			-- add to current group
			table.insert(groups[curGroupIndex], words[i])
		else
			-- start new group
			curGroupIndex = curGroupIndex + 1
			groups[curGroupIndex] = { words[i] }
		end
		curInCap = wordIsCap
	end

	return groups
end

function DumpTissueDamage(unit)
	Append ("PART" ..  " TISSUE" ..  " CUT" ..  " DENT" ..  " EFFECT" .. " WOUND AREA")
	for wI, wound in pairs(unit.body.wounds) do
		for wpI, wp in pairs(wound.parts) do
			bpI = wp.body_part_id
			layerI = wp.layer_idx
			gLayerI = wp.global_layer_idx

			bp = unit.body.body_plan.body_parts[bpI]
			bpName = bp.name_singular[0].value
			layer = bp.layers[layerI]
			layerName = layer.layer_name
	
			cut = unit.body.components.layer_cut_fraction[gLayerI]
			dent = unit.body.components.layer_dent_fraction[gLayerI]
			effect = unit.body.components.layer_effect_fraction[gLayerI]
	
			woundArea = unit.body.components.layer_wound_area[gLayerI]
	
			Append(bpName .. " " .. layerName .. " " ..  cut .. " " ..  dent .. " " ..  effect .. " " ..  woundArea)
		end
	end
end

function GetAttackerName(text)
	return string.match(text, "^(.-) in")
end

function GetDefenderName(text)
	return string.match(text, "^.- %a+ [T|t]he (.-) in")
end

function GetWeapon(text)
	return string.match(text, "with %a+ (.-),")
end

function GetFlavorText(text)
	return string.match(text, ", (.+)")
end


DumpedUnits = {}
ReportTexts = {}


--Append("COMBAT SNIFFER SESSION START")
--RegisterForEvents()

local usage = [====[

combat-sniffer
======
Collect data about combat strikes and write to combat-sniffer-log.txt

Usage examples::
	combat-sniffer -start 				start an unnamed session
	combat-sniffer -start "<name of session>"	start a named session

]====]

local utils = require 'utils'

validArgs = validArgs or utils.invert({
 'help',
 'start'
})

local args = utils.processArgs({...}, validArgs)

if args.help then
 print(usage)
 return
end


if(args.start) then
	if(args.start == "") then
		Append("COMBAT_SNIFFER_SESSION_START")
	else
		Append("COMBAT_SNIFFER_SESSION_START: " .. args.start)
	end
	RegisterForEvents()
else
	print (usage)
end





