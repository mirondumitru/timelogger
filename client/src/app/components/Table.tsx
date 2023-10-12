import React, { useMemo } from "react";
import { Column, useTable } from "react-table";

type Props = {
    data: any[],
    columns: Column[],
};

export default function TimeLoggerTable(props: Props) {

    const data = useMemo(() => props.data, [props.data]);
    const columns = useMemo(() => props.columns, [props.columns]);

    const { getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
    } = useTable({ columns, data });

    return (
        <table className="table-fixed w-full">
            <thead className="bg-gray-200">
                {headerGroups.map((headerGroup) => (
                    <tr {...headerGroup.getHeaderGroupProps()}>
                        {headerGroup.headers.map((column) => (
                            <th {...column.getHeaderProps({style: { minWidth: column.minWidth, width: column.width }})} scope="col" className="border px-4 py-2">
                                {column.render("Header")}
                            </th>
                        ))}
                    </tr>
                ))}
            </thead>
            <tbody {...getTableBodyProps()}>
                {rows.map((row) => {
                    prepareRow(row);
                    return (
                        <tr {...row.getRowProps()}>
                            {row.cells.map((cell) => {
                                return <td {...cell.getCellProps({style: { minWidth: cell.column.minWidth, width: cell.column.width }})} className="border px-4 py-2">{cell.render("Cell")}</td>;
                            })}
                        </tr>
                    );
                })}
            </tbody>
        </table>
    );
}
