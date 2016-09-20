--Calculates combat info for weapons/armor/creature. DFHack 0.40.08 version

unit=dfhack.gui.getSelectedUnit()
if unit==nil then
	 print ("No unit under cursor!  Aborting!")
	 return
	 end
current_year=df.global.cur_year
birth_year = unit.relations.birth_year
age = current_year - birth_year
cur_size = unit.body.size_info.size_cur

print ("Current Year/Birth Year/Age", current_year, birth_year, age)

race=df.global.world.raws.creatures.all[unit.race]
print("Creature size (base/current/racial): ", unit.body.size_info.size_base, unit.body.size_info.size_cur, race.adultsize) 
if unit.curse.attr_change ~= nil then
	curstrength=((unit.body.physical_attrs.STRENGTH.value * unit.curse.attr_change.phys_att_perc.STRENGTH)/100 + 

unit.curse.attr_change.phys_att_add.STRENGTH)
	if curstrength > 5000 then curstrength=5000 end
else
	curstrength=unit.body.physical_attrs.STRENGTH.value
end
print("Creature strength (base/current): ", unit.body.physical_attrs.STRENGTH.value, curstrength)
print("Wrestle/Charge rating: ", math.floor(curstrength/100+unit.body.size_info.size_cur/100))


print(" ") 
print("ITEMS") 
print(" ") 

for k,v in pairs(unit.inventory) do

--print(v.mode)
--enum-item	Hauled	0
--enum-item	Weapon	1
--enum-item	Worn	2
--enum-item	InBody	3
--enum-item	Flask	4
--enum-item	WrappedAround	5
--enum-item	StuckIn	6
--enum-item	InMouth	7
--enum-item	Shouldered	8
--enum-item	SewnInto	9

vitype=df.item_type[v.item:getType()]
--print(vitype)				-- why are all items printed??
material=dfhack.matinfo.decode(v.item)
matdata=material.material.strength
vmatname=material.material.state_name.Solid
--print(vmatname, v.item.subtype.name) --WOULD ENABLE THIS BUT BUG ON QUIVERS, OTHER ITEMS W/O SUBTYPES!

vbpart=unit.body.body_plan.body_parts[v.body_part_id]
--print(vbpart.name_singular[0].value)	-- why are all items printed??

if vitype=="WEAPON" then
	print(vmatname, v.item.subtype.name)
	v.item:calculateWeight()
	effweight=unit.body.size_info.size_cur/100+v.item.weight*100+v.item.weight_fraction/10000
	actweight=v.item.weight*1000+v.item.weight_fraction/1000
	if v.item.subtype.flags.HAS_EDGE_ATTACK==true then
		print("shear yield/fracture: ", matdata.yield.SHEAR, matdata.fracture.SHEAR)
		print("Sharpness: ", v.item.sharpness)
	end
	print("NAME", "EDGE", "CONTACT", "PNTRT", "WEIGHT", "VEL", "MOMENTUM(+500%/-50%)")
		for kk,vv in pairs(v.item.subtype.attacks) do
			vvel=unit.body.size_info.size_base * curstrength * vv.velocity_mult/1000/effweight/1000
			vmom=vvel*actweight/1000+1
			vedge="blunt"
			vcut=""
			if vv.edged==true then
				vedge="edged" 
				vcut=100
			end
			print(vv.verb_2nd, vedge, vv.contact, vv.penetration, actweight/1000, math.floor(vvel), math.floor(vmom))
		end
	actvol=v.item:getVolume()
	print("Blunt deflect if layer weight more than:", actvol * matdata.yield.IMPACT / 100 / 500 / 1000)

else
	if v.mode==1 then
		--item held in hands treated as misc weapon
		--1000 velocity mod, power math for contact and penetration
		print(vmatname, "(misc weapon)") --v.item.subtype.name quiver bug
		actvol=v.item:getVolume()
		v.item:calculateWeight()
		actweight=v.item.weight*1000+v.item.weight_fraction/1000
		effweight=unit.body.size_info.size_cur/100+v.item.weight*100+v.item.weight_fraction/10000
		misccontact=math.floor(actvol ^ 0.666)
		miscpene=math.floor((actvol*10000) ^ 0.333)
		print("NAME", "EDGE", "CONTACT", "PNTRT", "WEIGHT", "VEL", "MOMENTUM(+500%/-50%)")
		vvel=unit.body.size_info.size_base * curstrength/effweight/1000
		vmom=vvel*actweight/1000+1
		vedge="blunt"
		print("strike", vedge, misccontact, miscpene, actweight/1000, math.floor(vvel), math.floor(vmom))
		print("Blunt deflect if layer weight more than:", actvol * matdata.yield.IMPACT / 100 / 500 / 1000)
		print(" ")
	end
end


if vitype=="ARMOR" or vitype=="HELM" or vitype=="GLOVES" or vitype=="SHOES" or vitype=="PANTS" then
	print(vmatname, v.item.subtype.name)
	actvol=v.item:getVolume()
	v.item:calculateWeight()
	actweight=v.item.weight*1000+v.item.weight_fraction/1000
	vbca=actvol*matdata.yield.IMPACT/100/500/10
	vbcb=actvol*(matdata.fracture.IMPACT-matdata.yield.IMPACT)/100/500/10
	vbcc=actvol*(matdata.fracture.IMPACT-matdata.yield.IMPACT)/100/500/10
	deduct=vbca/10
	if matdata.strain_at_yield.IMPACT >= 50000 or v.item.subtype.props.flags.STRUCTURAL_ELASTICITY_WOVEN_THREAD==true or 

v.item.subtype.props.flags.STRUCTURAL_ELASTICITY_CHAIN_METAL==true or 

v.item.subtype.props.flags.STRUCTURAL_ELASTICITY_CHAIN_ALL==true then
		vbcb=0
		vbcc=0
	end
	print("Full contact blunt momentum resist: ", math.floor(vbca+vbcb+vbcc))
	print("Contact 10 blunt momentum resist: ", math.floor((vbca+vbcb+vbcc)*10/actvol))
	print("Unbroken momentum deduction (full,10): ", math.floor(deduct), math.floor(deduct*10/actvol))
	print("Volume/contact area/penetration: ", actvol)	
	print("Weight: ", actweight/1000)
	vshyre=matdata.yield.SHEAR
	vshfre=matdata.fracture.SHEAR
	if v.item.subtype.props.flags.STRUCTURAL_ELASTICITY_WOVEN_THREAD==true and vmatname ~= "leather" then
		if vshyre>20000 then vshyre=20000 end
		if vshfre>30000 then vshfre=30000 end
	end
	print("shear yield/fracture: ", vshyre, vshfre)
end

print(" ")
end    --end of unit inventory loop


print("BODY PART ATTACKS")
print("NAME", "EDGE", "SIZE", "CONTACT", "PNTRT", "WEIGHT", "VEL", "MOMENTUM(+500%/-50%)", "SOLID DENSITY")

for k,v in pairs(unit.body.body_plan.attacks) do
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
	print(v.name, vedge, partsize, contact, attpene, partweight/1000, math.floor(vvel), math.floor(vmom), 

material.material.state_name.Solid, material.material.solid_density)
	if v.flags.edge==true then
		print(" "," ","shear yield/fracture: ", matdata.yield.SHEAR, matdata.fracture.SHEAR)
	end
	--print(" ")
end


print(" ")
print("BODY PART DEFENSE")
print("Volume/Contact/Thickness/Material/Blunt_Momentum_Resist/Shear_Yield/Frac/PSize/PThick/PRelSize/BodyTotRelSize")

forTot = 0
for k,v in pairs(unit.body.body_plan.body_parts) do
	if (v.flags.SMALL==false and v.flags.INTERNAL==false) or v.flags.TOTEMABLE==true or false then

		partrelsize = v.relsize
		forTot = forTot + partrelsize
		bodyTotalRelSize = unit.body.body_plan.total_relsize
		partsize = math.floor(cur_size * v.relsize / unit.body.body_plan.total_relsize)
		partthick = math.floor((partsize * 10000) ^ 0.333)
		contact = math.floor(partsize ^ 0.666)

		print(v.name_singular[0].value)

		for kk,vv in pairs(v.layers) do

			tisdata=race.tissue[vv.tissue_id]
			layername = vv.layer_name
			
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


			print(" ",vv.layer_name, layervolume, contact, 
layerthick,material.material.state_name.Solid,fullbmr,matdata.yield.SHEAR,matdata.fracture.SHEAR, partsize, partthick, partrelsize, bodyTotalRelSize)
			
		end
	end
end
print ("For tot: ", forTot)
