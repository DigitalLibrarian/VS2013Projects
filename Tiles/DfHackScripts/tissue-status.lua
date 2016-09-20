unit=dfhack.gui.getSelectedUnit()
if unit==nil then
	 print ("No unit under cursor!  Aborting!")
	 return
	 end
print ("PART", "TISSUE", "CUT", "DENT", "EFFECT", "WOUND AREA")
for wI, wound in pairs(unit.body.wounds) do
	for wpI, wp in pairs(wound.parts) do
		bpI = wp.body_part_id
		layerI = wp.layer_idx
		gLayerI = wp.global_layer_idx

		bp = unit.body.body_plan.body_parts[bpI]
		layer = bp.layers[layerI]

		cut = unit.body.components.layer_cut_fraction[gLayerI]
		dent = unit.body.components.layer_dent_fraction[gLayerI]
		effect = unit.body.components.layer_effect_fraction[gLayerI]

		woundArea = unit.body.components.layer_wound_area[gLayerI]

		print (bp.name_singular[0].value, layer.layer_name, cut, dent, effect, woundArea)
	end


end
