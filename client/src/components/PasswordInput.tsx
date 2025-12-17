import { useState } from "react";
import eye from "../assets/eye.png";
import eyeOff from "../assets/eye-off.png";

type Props = {
    value: string;
    onChange: (value: string) => void;
    className?: string;
    required?: boolean;
};

export default function PasswordInput({
                                          value,
                                          onChange,
                                          className = "",
                                          required = true,      // ✅ default required
                                      }: Props) {
    const [visible, setVisible] = useState(false);

    const tooShort = value.length > 0 && value.length < 6;

    return (
        <div className="space-y-1">
            <label className="block text-sm font-medium text-black">
                Password
            </label>

            <div className="relative">
                <input
                    name="password"
                    type={visible ? "text" : "password"}
                    value={value}
                    required={required}
                    onChange={(e) => onChange(e.target.value)}
                    className={`input input-bordered w-full pr-12 z-0 ${
                        tooShort ? "border-red-500" : ""
                    } ${className}`}
                />

                <button
                    type="button"
                    className="absolute right-3 top-1/2 -translate-y-1/2"
                    onClick={() => setVisible(prev => !prev)}
                >
                    <img
                        src={visible ? eyeOff : eye}
                        alt="Toggle password visibility"
                        className="w-5 h-5 opacity-70 hover:opacity-100"
                    />
                </button>
            </div>

            {tooShort && (
                <p className="text-xs text-red-600">
                    Minimum 6 characters
                </p>
            )}
        </div>
    );
}
