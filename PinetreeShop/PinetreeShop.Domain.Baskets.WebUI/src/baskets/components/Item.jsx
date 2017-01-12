import React from 'react';
import { Button } from 'react-bootstrap';

const Item = (item, onChangeQuantity) => (
    <tr key={item.productId}>
        <td>{item.productName}</td>
        <td>{item.price}</td>
        <td>{item.quantity}</td>
        <td>
            <Button onClick={onChangeQuantity}>Change</Button>
        </td>
    </tr>
);

export default Item;