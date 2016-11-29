import React from 'react';
import { Table } from 'react-bootstrap';

const OrderLines = ({ orderLines }) => {
    let orderLineRows = orderLines.map(ol => (
        <tr key={ol.productId}>
            <td>{ol.productName}</td>
            <td>{ol.price}</td>
            <td>{ol.quantity}</td>
            <td>{ol.quantity * ol.price}</td>
        </tr>
    ));

    return (
        <Table>
            <thead>
                <tr>
                    <th>Product</th>
                    <th>Price</th>
                    <th>Quantity</th>
                    <th>Total</th>
                </tr>
            </thead>
            <tbody>
                {orderLineRows}
            </tbody>
        </Table>
    );
}

export default OrderLines;