/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./**/*.{razor,html,cshtml}"
    ],
    theme: {
        extend: {
            spacing: {
                '1/10': '10%',
                '2/10': '20%',
                '1/5': '20%',
            },
        },
    },
    variants: {
        extend: {
            width: ['responsive', 'hover', 'focus'],
        },
    },
    plugins: [
        // - Comfortaa font -------------------------------------------------------------------------------------------- 
        function ({addUtilities, theme, e}) {
            const newUtilities = {};

            newUtilities[`.font-comfortaa`] = {
                "font-family": `"Comfortaa" , sans-serif`,
                "font-optical-sizing": "auto",
                "font-style": `normal`,
            };

          addUtilities(newUtilities, ['responsive', 'hover']);
            
        },
        
        // - Debug Colors ----------------------------------------------------------------------------------------------
        function({ addUtilities, theme, e}) {
            const colors = theme('colors');
            const newUtilities = {};
      
            Object.keys(colors).forEach(color => {
                if (typeof colors[color] === 'string') {
                    newUtilities[`.DEBUG-${e(color)}`] = {
                        "border-style": "solid",
                        "border-width": "1px",
                        "border-color": colors[color],
                    };
                } else {
                    Object.keys(colors[color]).forEach(shade => {
                        newUtilities[`.DEBUG-${e(color)}-${shade}`] = {
                            "border-style": "solid",
                            "border-width": "1px",
                            "border-color": colors[color][shade],
                        };
                    });
                }
            });
      
            addUtilities(newUtilities, ['responsive', 'hover']);
        }
    ],
}

