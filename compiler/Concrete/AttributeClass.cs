using System;
using System.Collections.Generic;

namespace Concrete.AttributeClassSpace {
    /// <summary>
    /// use like table for OneSymbolSeparate symbols
    /// </summary>
    public static class AttributeClass {
        public static readonly UInt16
            ERROR = 0,
            WHITE_SPACE = 1,
            SEPARATOR = 2,
            WORD = 4,
            DIGIT = 8,
            COMMENT_STARTER_ENDER = 16, //symbol *
            COMMENT_END_BRACKET = 32,	//symbol )
            COMMENT_START_BRACKET = 64, //symbol (
            DOLLAR_SIGN = 128,
            HASHTAG = 256,
            SIGN = 512;

        static List<UInt16> attributes = new List<UInt16>();

        static AttributeClass() {
            for (byte ind = 0; ind < 255; ind++) {
				if (ind >= 'A' && ind <= 'Z' || ind >= 'a' && ind <= 'z') {
					//keyword or identifier
					attributes.Add ((UInt16)(WORD));
				} else if (ind >= '0' && ind <= '9') {
					//constant
					attributes.Add ((UInt16)(DIGIT));
				} else if (ind == '+' || ind == '-') {
                    attributes.Add((UInt16)(SEPARATOR | SIGN));
                } else if (ind == '#') {
                    attributes.Add((UInt16)(HASHTAG));
                } else if (ind == ':' || ind == ';' || ind == '=' || ind == ',' || ind == '!'
				           || ind == '/' || ind == '&' || ind == '^'
				           || ind == '[' || ind == ']' || ind == '\'' || ind == '"' || ind == '.'
				           || ind == '<' || ind == '>') {
					//separators
					attributes.Add ((UInt16)(SEPARATOR));
				} else if (ind == ' ' || ind == '\n' || ind == '\t' || ind == '\v' || ind == 12 || ind == '\r') {
					//white spaces
					attributes.Add ((UInt16)(WHITE_SPACE));
				} else if (ind == '*') {
					//may be comment start
					attributes.Add ((UInt16)(COMMENT_STARTER_ENDER | SEPARATOR));
				} else if (ind == ')') {
					//may be comment end
					attributes.Add ((UInt16)(COMMENT_END_BRACKET | SEPARATOR));
				} else if (ind == '(') {
					//may be comment start
					attributes.Add ((UInt16)(COMMENT_START_BRACKET | SEPARATOR));
				} else if (ind == '$') {
					//dollar sign
					attributes.Add ((UInt16)(DOLLAR_SIGN));
				}
                else {
					//just error symbol
                    attributes.Add((UInt16)(ERROR));
                }
            }
        }

        public static UInt16 Get(int index) {
			// get index attribute
            return AttributeClass.attributes[index];
        }
    }
}
