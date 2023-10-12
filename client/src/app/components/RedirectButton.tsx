import React from 'react';
import { useNavigate } from 'react-router-dom';

type Props = {
    text: string,
    path: string,
    disabled: boolean
};

export default function RedirectButton(props: Props) {
    const navigate = useNavigate();

    const navigateTo = () => {
        navigate(props.path);
    };

    return (
        <button className={!props.disabled ? "text-white font-bold py-2 px-4 rounded bg-blue-500 hover:bg-blue-700" : "text-white font-bold py-2 px-4 rounded bg-blue-100"} onClick={navigateTo} disabled={props.disabled}>
            {props.text}
        </button>
    );
}