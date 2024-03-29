﻿namespace AlbionProfit.Game.Items;

public static class CommonItems
{
    public static readonly Dictionary<RefinedResource, string[]> Resources =
        new Dictionary<RefinedResource, string[]>()
        {
            {
                RefinedResource.Leather, new[]
                {
                    "T2_HIDE", "T3_HIDE", "T4_HIDE", "T5_HIDE", "T6_HIDE", "T7_HIDE", "T8_HIDE"
                }
            },
            {
                RefinedResource.Cloth, new[]
                {
                    "T2_FIBER", "T3_FIBER", "T4_FIBER", "T5_FIBER", "T6_FIBER", "T7_FIBER", "T8_FIBER"
                }
            },
            {
                RefinedResource.Plank, new[]
                {
                    "T2_WOOD", "T3_WOOD", "T4_WOOD", "T5_WOOD", "T6_WOOD", "T7_WOOD", "T8_WOOD"
                }
            },
            {
                RefinedResource.MetalBar, new[]
                {
                    "T2_ORE", "T3_ORE", "T4_ORE", "T5_ORE", "T6_ORE", "T7_ORE", "T8_ORE"
                }
            },
            {
                RefinedResource.StoneBlocks, new[]
                {
                    "T2_ROCK", "T3_ROCK", "T4_ROCK", "T5_ROCK", "T6_ROCK", "T7_ROCK", "T8_ROCK"
                }
            }
        };
    
    public static readonly Dictionary<RefinedResource, string[]> RefinedResources =
        new Dictionary<RefinedResource, string[]>()
        {
            {
                RefinedResource.Leather, new[]
                {
                    "T2_LEATHER", "T3_LEATHER", "T4_LEATHER", "T5_LEATHER", "T6_LEATHER", "T7_LEATHER", "T8_LEATHER"
                }
            },
            {
                RefinedResource.Cloth, new[]
                {
                    "T2_CLOTH", "T3_CLOTH", "T4_CLOTH", "T5_CLOTH", "T6_CLOTH", "T7_CLOTH", "T8_CLOTH"
                }
            },
            {
                RefinedResource.Plank, new[]
                {
                    "T2_PLANKS", "T3_PLANKS", "T4_PLANKS", "T5_PLANKS", "T6_PLANKS", "T7_PLANKS", "T8_PLANKS"
                }
            },
            {
                RefinedResource.MetalBar, new[]
                {
                    "T2_METALBAR", "T3_METALBAR", "T4_METALBAR", "T5_METALBAR", "T6_METALBAR", "T7_METALBAR", "T8_METALBAR"
                }
            },
            {
                RefinedResource.StoneBlocks, new[]
                {
                    "T2_STONEBLOCK", "T3_STONEBLOCK", "T4_STONEBLOCK", "T5_STONEBLOCK", "T6_STONEBLOCK", "T7_STONEBLOCK", "T8_STONEBLOCK"
                }
            }
        };
}