
function RegisterforReports()
	eventful.onReport.something=function(reportId)
	    local report=df.report.find(reportId)
	        print("type:",report.type)
	        print("text:",report.text)
	        print("color:",report.color)
	        print("bright:",report.bright)
	        print("duration:",report.duration)
	        print("flags:",report.flags)  -- bitfield
	        print("flags.whole:",report.flags.whole)
	        print("repeat_count:",report.repeat_count)
	        print("pos:",report.pos)
	        print("id:",report.id)
	        print("year:",report.year)
	        print("time:",report.time)
	        print("unk_v40_1:",report.unk_v40_1)
	        print("unk_v40_2:",report.unk_v40_2)
	        print("unk_v40_3:",report.unk_v40_3)
	end
end



function Append(str)
	fileName = "test.txt"
	file = io.open(fileName, "a")
	io.output(file)
	io.write(str)
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


text = "The Stray Yak Cow kicks The Giant Bat in the upper body with her left rear hoof, bruising the muscle and shattering the left floating rib"
groups = GetCapitalizedGroups(text)

for i=1, #groups do
	print (groups[i])
	for j =1, TableLength(groups[i]) do
		print (groups[i][j])
	end
end
--
--

attackerGroup = groups[1]
verbGroup = groups[2]
defenderGroup = groups[3]
resultsGroup = groups[4]

attacker = table.concat(attackerGroup, " ")
verb = table.concat(verbGroup, " ")
defender = table.concat(defenderGroup, " ")
results = table.concat(resultsGroup, " ")


print ("Attacker: ", attacker)
print ("Verb: ", verb)
print ("Defender: ", defender)
print ("Results: ", results)







