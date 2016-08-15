using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Structures;
using Tiles.Math;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class StructureFactory
    {
        public IStructure CreateRectangularBuilding(Vector3 size, Color buildingColor = Color.Gray, Color notBuildingColor = Color.Black)
        {
            var box = new Box3(Vector3.Zero, size);
            var structure = new Structure("Rectangular Building", size);
            IStructureCell cell;
            for (int x = box.Min.X; x <= box.Max.X; x++)
            {
                for (int y = box.Min.Y; y <= box.Max.Y; y++)
                {
                    for (int z = box.Min.Z; z <= box.Max.Z; z++)
                    {
                        cell = null;
                        if (x == box.Min.X && y == box.Min.Y)
                        {
                            cell = new StructureCell(
                                structure,
                                StructureCellType.Corner_TopLeft,
                                new Sprite(Symbol.Wall_TopLeft_L_Hollow, buildingColor, notBuildingColor),
                                canPass: false);
                        }
                        else if (x == box.Max.X && y == box.Min.Y)
                        {
                            cell = new StructureCell(
                                structure,
                                StructureCellType.Corner_TopRight,
                                new Sprite(Symbol.Wall_TopRight_L_Hollow, buildingColor, notBuildingColor),
                                canPass: false);
                        }
                        else if (x == box.Min.X && y == box.Max.Y)
                        {
                            cell = new StructureCell(
                                structure,
                                StructureCellType.Corner_BottomLeft,
                                new Sprite(Symbol.Wall_BottomLeft_L_Hollow, buildingColor, notBuildingColor),
                                canPass: false);
                        }
                        else if (x == box.Max.X && y == box.Max.Y)
                        {
                            cell = new StructureCell(
                                structure,
                                StructureCellType.Corner_BottomRight,
                                new Sprite(Symbol.Wall_BottomRight_L_Hollow, buildingColor, notBuildingColor),
                                canPass: false);
                        }
                        else if (x == box.Min.X || x == box.Max.X)
                        {
                            cell = new StructureCell(
                                structure,
                                StructureCellType.Wall_Vertical,
                                new Sprite(Symbol.Wall_Vertical_Hollow, buildingColor, notBuildingColor),
                                canPass: false);
                        }
                        else if (y == box.Min.Y || y == box.Max.Y)
                        {
                            cell = new StructureCell(
                                structure,
                                StructureCellType.Wall_Horizontal,
                                new Sprite(Symbol.Wall_Horizontal_Hollow, buildingColor, notBuildingColor),
                                canPass: false);
                        }
                        else if(z == box.Min.Z)
                        {
                            cell = new StructureCell(
                                structure,
                                StructureCellType.Floor,
                                new Sprite(Symbol.Terrain_Floor, notBuildingColor, buildingColor),
                                canPass: true);
                        }
                        else if (z == box.Max.Z)
                        {
                            cell = new StructureCell(
                                structure,
                                StructureCellType.Floor,
                                new Sprite(Symbol.Terrain_Floor, notBuildingColor, buildingColor),
                                canPass: true);
                        }

                        if (cell != null)
                        {
                            var pos = new Vector3(x, y, z);
                            var relPos = pos - box.Min;
                            structure.Add(relPos, cell);
                        }
                    }
                }
            }

            return structure;
        }
        public IStructure CreateRectangularBuilding(Vector3 size, CompassDirection door, Color fg = Color.Gray, Color bg = Color.Black)
        {
            var structure = CreateRectangularBuilding(size);
            
            var box = new Box3(Vector3.Zero, size);
            AddDoor(structure, box, door, fg, bg);

            return structure;
        }
        void AddDoor(IStructure structure, Box3 box, CompassDirection door, Color fg = Color.Gray, Color bg = Color.Black)
        {
            IStructureCell cell = null;
            // add door to middle of the specified wall

            int groundFloor = box.Min.Z;
            switch (door)
            {
                case CompassDirection.North:
                    cell = structure.Cells[new Vector3(box.Size.X / 2, 0, groundFloor)];
                    break;
                case CompassDirection.South:
                    cell = structure.Cells[new Vector3(box.Size.X / 2, box.Max.Y, groundFloor)];
                    break;
                case CompassDirection.West:
                    cell = structure.Cells[new Vector3(0, box.Size.Y / 2, groundFloor)];
                    break;
                case CompassDirection.East:
                    cell = structure.Cells[new Vector3(box.Max.X, box.Size.Y / 2, groundFloor)];
                    break;
                default: throw new InvalidOperationException(string.Format("Need door tile defined for CompassDirection={0}", door));
            }

            // TODO - make working doors
            cell.Type = StructureCellType.Gizmo;
            cell.Sprite = new Sprite(Symbol.None, fg, bg);
            cell.CanOpen = true;
            cell.CanClose = true;
            cell.CanPass = true;
            cell.IsOpen = true;

        }

    }
}
